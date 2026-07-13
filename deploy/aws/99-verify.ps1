# =====================================================================
# 99-verify.ps1  —  Post-deploy smoke tests + teardown reference.
# =====================================================================
$ErrorActionPreference = "Continue"
if (-not $Global:BAU_REGION) { . "$PSScriptRoot\00-config.ps1" }
$o = Get-Content "$PSScriptRoot\.outputs.json" -Raw | ConvertFrom-Json
$base = "http://$($o.albDns)"

Write-Host "==> Waiting for ECS services to stabilize..." -ForegroundColor Cyan
aws ecs wait services-stable --cluster $BAU_CLUSTER --services $BAU_SVC_REGISTRY $BAU_SVC_HR --region $BAU_REGION

Write-Host "==> Verifying endpoints at $base" -ForegroundColor Cyan
function Try-Get($label, $url) {
	try { $r = Invoke-RestMethod $url -TimeoutSec 10; Write-Host "  OK  $label" -ForegroundColor Green; return $r }
	catch { Write-Host "  ERR $label -> $($_.Exception.Message)" -ForegroundColor Red }
}
Try-Get "registry /health"            "$base/health"                          | Out-Null
$mods = Try-Get "registry /api/modules" "$base/api/modules"
if ($mods) { Write-Host "      modules=$($mods.Count)" }
$spec = Try-Get "HR mobile ui-spec"   "$base/api/hr/mobile/ui-spec"
if ($spec) { Write-Host "      nav=$($spec.navigation.items.Count) screens=$($spec.screens.PSObject.Properties.Name.Count)" }
Try-Get "HR employee-list data"       "$base/api/hr/mobile/data/employee-list" | Out-Null

Write-Host "`n==> Persistence test: restart HR task, confirm module still present" -ForegroundColor Cyan
Write-Host "    (RDS-backed registry should keep the module across restarts.)"

Write-Host "`n--- TEARDOWN (run when demo is done to stop charges) ---" -ForegroundColor DarkYellow
Write-Host "aws ecs update-service --cluster $BAU_CLUSTER --service $BAU_SVC_REGISTRY --desired-count 0 --region $BAU_REGION"
Write-Host "aws ecs update-service --cluster $BAU_CLUSTER --service $BAU_SVC_HR --desired-count 0 --region $BAU_REGION"
Write-Host "aws ecs delete-service --cluster $BAU_CLUSTER --service $BAU_SVC_REGISTRY --force --region $BAU_REGION"
Write-Host "aws ecs delete-service --cluster $BAU_CLUSTER --service $BAU_SVC_HR --force --region $BAU_REGION"
Write-Host "aws elbv2 delete-load-balancer --load-balancer-arn $($o.albArn) --region $BAU_REGION"
Write-Host "aws rds delete-db-instance --db-instance-identifier $BAU_RDS_ID --skip-final-snapshot --region $BAU_REGION"
Write-Host "aws ecs delete-cluster --cluster $BAU_CLUSTER --region $BAU_REGION"
