# BusinessAsUsual — AWS Deploy Assets

Ready-to-run assets to host **ModuleRegistry.API** and **HR.API** on **AWS ECS Fargate**
behind an **Application Load Balancer**, backed by **Amazon RDS SQL Server**, in the
**default VPC** of `us-east-1`.

> These were generated because the CLI identity currently configured
> (`bau-github-deploy`) lacks the IAM permissions to provision infrastructure
> (ECR/ECS/RDS/ALB/IAM). Run them with an **admin** profile.

## Files
| File | Purpose |
|------|---------|
| `00-config.ps1` | Central variables (account, region, names, sizes). Dot-source before others. |
| `01-push-images.ps1` | Create ECR repos, build both images, push `:latest`. |
| `02-provision-infra.ps1` | Security groups, RDS SQL Server, ALB + target groups + listener rules, SSM params, ECS cluster + IAM role. |
| `03-deploy-services.ps1` | Render task defs, register them, create/update ECS services. |
| `taskdef.registry.json` | ECS task definition template for ModuleRegistry.API. |
| `taskdef.hr.json` | ECS task definition template for HR.API. |
| `99-verify.ps1` | Post-deploy smoke tests (health, /api/modules, mobile ui-spec, persistence). |

## Prerequisites
- AWS CLI v2 authenticated as an **admin** (or a role with ECR/ECS/RDS/EC2/ELBv2/IAM/SSM rights):
  ```powershell
  aws configure --profile bau-admin
  $env:AWS_PROFILE = "bau-admin"
  ```
- Docker Desktop running (Linux engine).
- Run from the **repo root** so Docker build contexts resolve.

## Order
```powershell
. .\deploy\aws\00-config.ps1
.\deploy\aws\01-push-images.ps1
.\deploy\aws\02-provision-infra.ps1   # RDS creation takes ~10-20 min
.\deploy\aws\03-deploy-services.ps1
.\deploy\aws\99-verify.ps1
```

## After deploy
`02-provision-infra.ps1` prints the **ALB DNS name**. Put it in the Android app:
`app/build.gradle.kts` → `AWS_BASE_URL` / `AWS_REGISTRY_URL` (see the mobile failover section).
Because HR advertises its public URL via `HR__ApiBaseUrl` (an ECS env var set to the ALB URL),
mobile discovery returns reachable endpoints.

## Cost note
RDS SQL Server + ALB + 2 Fargate tasks incur ongoing charges. Tear down with the commands at
the bottom of `99-verify.ps1` when the demo is done.
