# =====================================================================
# 00-config.ps1  —  Central configuration. Dot-source this first:
#   . .\deploy\aws\00-config.ps1
# =====================================================================

# --- Identity / region ---
$Global:BAU_REGION  = "us-east-1"
$Global:BAU_ACCOUNT = (aws sts get-caller-identity --query Account --output text)

# --- Naming ---
$Global:BAU_CLUSTER      = "bau-cluster"
$Global:BAU_ALB          = "bau-alb"
$Global:BAU_TG_REGISTRY  = "bau-tg-registry"
$Global:BAU_TG_HR        = "bau-tg-hr"
$Global:BAU_ECR_REGISTRY = "bau-registry"
$Global:BAU_ECR_HR       = "bau-hr"
$Global:BAU_SVC_REGISTRY = "bau-registry-svc"
$Global:BAU_SVC_HR       = "bau-hr-svc"
$Global:BAU_LOG_GROUP    = "/ecs/bau"

# --- RDS (SQL Server Express, dev tier) ---
$Global:BAU_RDS_ID       = "bau-sqlserver"
$Global:BAU_RDS_CLASS    = "db.t3.small"
$Global:BAU_RDS_STORAGE  = 20
$Global:BAU_RDS_USER     = "bauadmin"
# Override this before running; do NOT commit a real password.
if (-not $Global:BAU_RDS_PASSWORD) { $Global:BAU_RDS_PASSWORD = "ChangeMe_Strong123!" }

# --- Fargate task sizing (dev tier) ---
$Global:BAU_TASK_CPU     = "256"   # 0.25 vCPU
$Global:BAU_TASK_MEM     = "512"   # 0.5 GB

# --- Derived: ECR image URIs ---
$Global:BAU_ECR_BASE     = "$Global:BAU_ACCOUNT.dkr.ecr.$Global:BAU_REGION.amazonaws.com"
$Global:BAU_IMG_REGISTRY = "$Global:BAU_ECR_BASE/$Global:BAU_ECR_REGISTRY:latest"
$Global:BAU_IMG_HR       = "$Global:BAU_ECR_BASE/$Global:BAU_ECR_HR:latest"

# --- SSM parameter names (connection strings live here, encrypted) ---
$Global:BAU_SSM_MRS_CONN = "/bau/registry/MRS_SQL_CONNECTION_STRING"
$Global:BAU_SSM_HR_CONN  = "/bau/hr/HR_SQL_CONNECTION_STRING"

Write-Host "BAU config loaded: account=$Global:BAU_ACCOUNT region=$Global:BAU_REGION" -ForegroundColor Cyan
Write-Host "ECR base: $Global:BAU_ECR_BASE" -ForegroundColor DarkGray
