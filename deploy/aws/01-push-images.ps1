# =====================================================================
# 01-push-images.ps1  —  Create ECR repos, build & push both images.
# Run from repo root:  .\deploy\aws\01-push-images.ps1
# =====================================================================
$ErrorActionPreference = "Stop"
if (-not $Global:BAU_REGION) { . "$PSScriptRoot\00-config.ps1" }

Write-Host "==> Creating ECR repositories (idempotent)..." -ForegroundColor Cyan
foreach ($repo in @($BAU_ECR_REGISTRY, $BAU_ECR_HR)) {
	aws ecr describe-repositories --repository-names $repo --region $BAU_REGION 2>$null | Out-Null
	if ($LASTEXITCODE -ne 0) {
		aws ecr create-repository --repository-name $repo --region $BAU_REGION `
			--image-scanning-configuration scanOnPush=true | Out-Null
		Write-Host "    created $repo"
	} else {
		Write-Host "    exists  $repo"
	}
}

Write-Host "==> Logging Docker in to ECR..." -ForegroundColor Cyan
aws ecr get-login-password --region $BAU_REGION |
	docker login --username AWS --password-stdin $BAU_ECR_BASE

# Build from repo root so the Dockerfile COPY paths resolve.
Push-Location (Resolve-Path "$PSScriptRoot\..\..")
try {
	Write-Host "==> Building & pushing ModuleRegistry image..." -ForegroundColor Cyan
	docker build -f services/ModuleRegistry/ModuleRegistry.API/Dockerfile -t $BAU_IMG_REGISTRY .
	docker push $BAU_IMG_REGISTRY

	Write-Host "==> Building & pushing HR image..." -ForegroundColor Cyan
	docker build -f services/HR/HR.API/Dockerfile -t $BAU_IMG_HR .
	docker push $BAU_IMG_HR
}
finally { Pop-Location }

Write-Host "==> Images pushed:" -ForegroundColor Green
Write-Host "    $BAU_IMG_REGISTRY"
Write-Host "    $BAU_IMG_HR"
