# =====================================================================
# 02-provision-infra.ps1
#   Default-VPC networking, RDS SQL Server, ALB + target groups,
#   SSM params, ECS cluster, IAM execution role, CloudWatch logs.
# Run with an ADMIN profile:  .\deploy\aws\02-provision-infra.ps1
# =====================================================================
$ErrorActionPreference = "Stop"
if (-not $Global:BAU_REGION) { . "$PSScriptRoot\00-config.ps1" }
function AWS { aws @args --region $BAU_REGION --output text }

# --- Default VPC + its subnets ---
Write-Host "==> Discovering default VPC + subnets..." -ForegroundColor Cyan
$vpc = AWS ec2 describe-vpcs --filters "Name=isDefault,Values=true" --query "Vpcs[0].VpcId"
$subnets = (AWS ec2 describe-subnets --filters "Name=vpc-id,Values=$vpc" --query "Subnets[].SubnetId") -split "\s+"
$subnetCsv = ($subnets -join ",")
Write-Host "    VPC=$vpc  subnets=$subnetCsv"

# --- Security groups: ALB (public 80), tasks (from ALB), RDS (1433 from tasks) ---
Write-Host "==> Creating security groups..." -ForegroundColor Cyan
function Ensure-Sg($name, $desc) {
	$id = AWS ec2 describe-security-groups --filters "Name=group-name,Values=$name" "Name=vpc-id,Values=$vpc" --query "SecurityGroups[0].GroupId"
	if (-not $id -or $id -eq "None") {
		$id = AWS ec2 create-security-group --group-name $name --description $desc --vpc-id $vpc --query "GroupId"
	}
	return $id
}
$sgAlb  = Ensure-Sg "bau-alb-sg"  "BAU ALB public ingress"
$sgTask = Ensure-Sg "bau-task-sg" "BAU Fargate tasks"
$sgRds  = Ensure-Sg "bau-rds-sg"  "BAU RDS SQL Server"

# Ingress rules (ignore errors if they already exist)
AWS ec2 authorize-security-group-ingress --group-id $sgAlb  --protocol tcp --port 80   --cidr 0.0.0.0/0 2>$null | Out-Null
AWS ec2 authorize-security-group-ingress --group-id $sgTask --protocol tcp --port 80   --source-group $sgAlb 2>$null | Out-Null
AWS ec2 authorize-security-group-ingress --group-id $sgRds  --protocol tcp --port 1433 --source-group $sgTask 2>$null | Out-Null
Write-Host "    alb=$sgAlb task=$sgTask rds=$sgRds"

# --- RDS SQL Server (Express, dev tier) ---
Write-Host "==> Creating RDS SQL Server '$BAU_RDS_ID' (may take 10-20 min)..." -ForegroundColor Cyan
$exists = AWS rds describe-db-instances --db-instance-identifier $BAU_RDS_ID --query "DBInstances[0].DBInstanceStatus" 2>$null
if (-not $exists -or $exists -eq "None") {
	AWS rds create-db-instance `
		--db-instance-identifier $BAU_RDS_ID `
		--db-instance-class $BAU_RDS_CLASS `
		--engine sqlserver-ex `
		--master-username $BAU_RDS_USER `
		--master-user-password $BAU_RDS_PASSWORD `
		--allocated-storage $BAU_RDS_STORAGE `
		--vpc-security-group-ids $sgRds `
		--no-multi-az --no-publicly-accessible `
		--backup-retention-period 1 | Out-Null
	Write-Host "    waiting for RDS to become available..."
	aws rds wait db-instance-available --db-instance-identifier $BAU_RDS_ID --region $BAU_REGION
}
$rdsEndpoint = AWS rds describe-db-instances --db-instance-identifier $BAU_RDS_ID --query "DBInstances[0].Endpoint.Address"
Write-Host "    RDS endpoint=$rdsEndpoint"

# --- Connection strings -> SSM Parameter Store (SecureString) ---
Write-Host "==> Storing connection strings in SSM..." -ForegroundColor Cyan
$mrsConn = "Server=$rdsEndpoint,1433;Database=BusinessAsUsual_ModuleRegistry;User Id=$BAU_RDS_USER;Password=$BAU_RDS_PASSWORD;TrustServerCertificate=True;"
$hrConn  = "Server=$rdsEndpoint,1433;Database=BusinessAsUsual_HR;User Id=$BAU_RDS_USER;Password=$BAU_RDS_PASSWORD;TrustServerCertificate=True;"
AWS ssm put-parameter --name $BAU_SSM_MRS_CONN --type SecureString --value $mrsConn --overwrite | Out-Null
AWS ssm put-parameter --name $BAU_SSM_HR_CONN  --type SecureString --value $hrConn  --overwrite | Out-Null

# --- ALB + target groups + listener with path routing ---
Write-Host "==> Creating Application Load Balancer..." -ForegroundColor Cyan
$albArn = AWS elbv2 describe-load-balancers --names $BAU_ALB --query "LoadBalancers[0].LoadBalancerArn" 2>$null
if (-not $albArn -or $albArn -eq "None") {
	$albArn = AWS elbv2 create-load-balancer --name $BAU_ALB --type application --scheme internet-facing `
		--subnets $subnets --security-groups $sgAlb --query "LoadBalancers[0].LoadBalancerArn"
	aws elbv2 wait load-balancer-available --load-balancer-arns $albArn --region $BAU_REGION
}
$albDns = AWS elbv2 describe-load-balancers --load-balancer-arns $albArn --query "LoadBalancers[0].DNSName"

function Ensure-Tg($name) {
	$arn = AWS elbv2 describe-target-groups --names $name --query "TargetGroups[0].TargetGroupArn" 2>$null
	if (-not $arn -or $arn -eq "None") {
		$arn = AWS elbv2 create-target-group --name $name --protocol HTTP --port 80 --vpc-id $vpc `
			--target-type ip --health-check-path "/health" `
			--health-check-interval-seconds 30 --healthy-threshold-count 2 `
			--query "TargetGroups[0].TargetGroupArn"
	}
	return $arn
}
$tgRegistry = Ensure-Tg $BAU_TG_REGISTRY
$tgHr       = Ensure-Tg $BAU_TG_HR

# Listener: default -> registry; /api/hr/* and /health-hr -> HR
$listenerArn = AWS elbv2 describe-listeners --load-balancer-arn $albArn --query "Listeners[0].ListenerArn" 2>$null
if (-not $listenerArn -or $listenerArn -eq "None") {
	$listenerArn = AWS elbv2 create-listener --load-balancer-arn $albArn --protocol HTTP --port 80 `
		--default-actions "Type=forward,TargetGroupArn=$tgRegistry" --query "Listeners[0].ListenerArn"
}
# Route HR traffic by path (registry stays default).
aws elbv2 create-rule --listener-arn $listenerArn --priority 10 --region $BAU_REGION `
	--conditions "Field=path-pattern,Values=/api/hr/*" `
	--actions "Type=forward,TargetGroupArn=$tgHr" 2>$null | Out-Null

# --- CloudWatch log group ---
aws logs create-log-group --log-group-name $BAU_LOG_GROUP --region $BAU_REGION 2>$null | Out-Null

# --- ECS cluster ---
Write-Host "==> Creating ECS cluster..." -ForegroundColor Cyan
AWS ecs create-cluster --cluster-name $BAU_CLUSTER 2>$null | Out-Null

# --- IAM execution role (pull from ECR, read SSM, write logs) ---
Write-Host "==> Ensuring ECS task execution role..." -ForegroundColor Cyan
$roleName = "bau-ecs-exec-role"
$roleArn = AWS iam get-role --role-name $roleName --query "Role.Arn" 2>$null
if (-not $roleArn -or $roleArn -eq "None") {
	$trust = '{"Version":"2012-10-17","Statement":[{"Effect":"Allow","Principal":{"Service":"ecs-tasks.amazonaws.com"},"Action":"sts:AssumeRole"}]}'
	$trust | Out-File -Encoding ascii "$env:TEMP\bau-trust.json"
	$roleArn = AWS iam create-role --role-name $roleName --assume-role-policy-document "file://$env:TEMP/bau-trust.json" --query "Role.Arn"
	aws iam attach-role-policy --role-name $roleName --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy | Out-Null
	aws iam attach-role-policy --role-name $roleName --policy-arn arn:aws:iam::aws:policy/AmazonSSMReadOnlyAccess | Out-Null
}

# --- Persist outputs for the next script ---
$outputs = [ordered]@{
	vpc=$vpc; subnets=$subnetCsv; sgTask=$sgTask; albArn=$albArn; albDns=$albDns
	tgRegistry=$tgRegistry; tgHr=$tgHr; execRoleArn=$roleArn; rdsEndpoint=$rdsEndpoint
}
$outputs | ConvertTo-Json | Out-File -Encoding ascii "$PSScriptRoot\.outputs.json"

Write-Host "`n==> INFRA READY" -ForegroundColor Green
Write-Host "    ALB DNS:  http://$albDns" -ForegroundColor Yellow
Write-Host "    Put this in app/build.gradle.kts AWS_BASE_URL / AWS_REGISTRY_URL" -ForegroundColor Yellow
