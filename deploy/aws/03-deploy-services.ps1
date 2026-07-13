# =====================================================================
# 03-deploy-services.ps1
#   Render task-def templates, register them, create/update ECS services.
# Run after 01 + 02:  .\deploy\aws\03-deploy-services.ps1
# =====================================================================
$ErrorActionPreference = "Stop"
if (-not $Global:BAU_REGION) { . "$PSScriptRoot\00-config.ps1" }
$o = Get-Content "$PSScriptRoot\.outputs.json" -Raw | ConvertFrom-Json

function Render($template, $out, $subs) {
	$text = Get-Content $template -Raw
	foreach ($k in $subs.Keys) { $text = $text.Replace($k, $subs[$k]) }
	$text | Out-File -Encoding ascii $out
}

$common = @{
	"__CPU__"           = $BAU_TASK_CPU
	"__MEM__"           = $BAU_TASK_MEM
	"__EXEC_ROLE_ARN__" = $o.execRoleArn
	"__LOG_GROUP__"     = $BAU_LOG_GROUP
	"__REGION__"        = $BAU_REGION
}

Write-Host "==> Registering task definitions..." -ForegroundColor Cyan
Render "$PSScriptRoot\taskdef.registry.json" "$env:TEMP\td-registry.json" ($common + @{
	"__IMG_REGISTRY__" = $BAU_IMG_REGISTRY
	"__SSM_MRS_CONN__" = "arn:aws:ssm:$($BAU_REGION):$($BAU_ACCOUNT):parameter$($BAU_SSM_MRS_CONN)"
})
Render "$PSScriptRoot\taskdef.hr.json" "$env:TEMP\td-hr.json" ($common + @{
	"__IMG_HR__"      = $BAU_IMG_HR
	"__ALB_DNS__"     = $o.albDns
	"__SSM_HR_CONN__" = "arn:aws:ssm:$($BAU_REGION):$($BAU_ACCOUNT):parameter$($BAU_SSM_HR_CONN)"
})

aws ecs register-task-definition --cli-input-json "file://$env:TEMP/td-registry.json" --region $BAU_REGION | Out-Null
aws ecs register-task-definition --cli-input-json "file://$env:TEMP/td-hr.json" --region $BAU_REGION | Out-Null

$netCfg = "awsvpcConfiguration={subnets=[$($o.subnets)],securityGroups=[$($o.sgTask)],assignPublicIp=ENABLED}"

function Deploy-Service($svc, $family, $tgArn, $container) {
	$exists = aws ecs describe-services --cluster $BAU_CLUSTER --services $svc --region $BAU_REGION --query "services[0].status" --output text 2>$null
	if ($exists -eq "ACTIVE") {
		Write-Host "    updating $svc..."
		aws ecs update-service --cluster $BAU_CLUSTER --service $svc --task-definition $family `
			--force-new-deployment --region $BAU_REGION | Out-Null
	} else {
		Write-Host "    creating $svc..."
		aws ecs create-service --cluster $BAU_CLUSTER --service-name $svc `
			--task-definition $family --desired-count 1 --launch-type FARGATE `
			--network-configuration $netCfg `
			--load-balancers "targetGroupArn=$tgArn,containerName=$container,containerPort=80" `
			--region $BAU_REGION | Out-Null
	}
}

Write-Host "==> Deploying ECS services..." -ForegroundColor Cyan
Deploy-Service $BAU_SVC_REGISTRY "bau-registry" $o.tgRegistry "registry"
Deploy-Service $BAU_SVC_HR       "bau-hr"       $o.tgHr       "hr"

Write-Host "`n==> SERVICES DEPLOYING. Public base URL:" -ForegroundColor Green
Write-Host "    http://$($o.albDns)" -ForegroundColor Yellow
Write-Host "    Registry: http://$($o.albDns)/api/modules"
Write-Host "    HR mobile: http://$($o.albDns)/api/hr/mobile/ui-spec"

# --- Auto-patch the Android app with the live ALB DNS ---
Write-Host "`n==> Patching Android AWS endpoints with live ALB DNS..." -ForegroundColor Cyan
try {
	& "$PSScriptRoot\set-alb-url.ps1" -AlbDns $o.albDns
} catch {
	Write-Host "    Skipped Android patch: $($_.Exception.Message)" -ForegroundColor Yellow
	Write-Host "    Run manually: .\deploy\aws\set-alb-url.ps1 -AlbDns $($o.albDns)" -ForegroundColor DarkGray
}
