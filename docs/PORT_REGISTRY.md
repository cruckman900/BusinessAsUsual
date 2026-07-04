# 🔌 Port Registry

> **Authoritative service → port map for local development.**
> Ports are sourced directly from each project's `Properties/launchSettings.json`.
> When you add a service, reserve a port here first to avoid collisions.

_Last verified against `launchSettings.json` files in the solution._

---

## 🟢 Active Services

| Service | Project | HTTP URL | Layer |
|---------|---------|----------|-------|
| **Platform Shell (Web)** | `frontend/BusinessAsUsual.Web` | http://localhost:5269 | UI (Blazor) |
| **Admin Portal** | `frontend/BusinessAsUsual.Admin` | http://localhost:5145 | UI (Blazor) |
| **Platform API** | `backend/BusinessAsUsual.API` | http://localhost:5000 | API |
| **Platform Core** | `BusinessAsUsual.Core` | http://localhost:5078 | Core/Host |
| **Module Registry API** | `services/ModuleRegistry/ModuleRegistry.API` | http://localhost:5100 | Service API |
| **HR Web** | `services/HR/HR.Web` | http://localhost:5002 | Module UI |
| **HR API** | `services/HR/HR.API` | http://localhost:5041 | Module API |
| **CRM Web** | `services/CRM/CRM.Web` | http://localhost:5005 | Module UI |
| **CRM API** | `services/CRM/CRM.API` | http://localhost:5004 | Module API |

## 🟡 Planned / Reserved

| Service | Suggested Port | Notes |
|---------|----------------|-------|
| **AI Microservice API** (`services/AI/AI.Api`) | http://localhost:5300 | `/api/ai/*` — see [ARCHITECTURE.md](ARCHITECTURE.md) & [AI-PriorityQueue.md](AI-PriorityQueue.md) |
| **Vector DB (Qdrant)** | http://localhost:6333 | Optional, for embeddings/RAG (Docker) |
| **API Gateway** | http://localhost:5200 | Future reverse-proxy / BFF |

---

## 📋 Port Allocation Convention

To keep the map predictable as modules grow, follow this banding:

| Band | Range | Purpose |
|------|-------|---------|
| `52xx` | 5200–5299 | Gateways / cross-cutting infrastructure |
| `53xx` | 5300–5399 | AI / ML services |
| `50xx` | 5000–5099 | Platform-level APIs & core |
| `51xx` | 5100–5199 | Module APIs & registry |
| `50xx` UI | 5002–5099 | Module & shell web front-ends |
| `514x` | 5140–5149 | Admin/back-office UIs |
| `63xx` | 6333+ | Vector DB / data infrastructure (Docker) |

> ⚠️ Some existing ports predate this convention (e.g., HR Web `5002`, HR API `5041`).
> They are kept as-is to avoid breaking existing launch profiles; new services
> should follow the banding above.

## 🧪 Quick Verify

Re-scan all launch profiles at any time:

```powershell
Get-ChildItem -Recurse -Filter launchSettings.json |
  Select-String -Pattern '"applicationUrl"' |
  Format-Table -AutoSize
```

See also: [MICROSERVICEARCHITECTUREOVERVIEW.md](MICROSERVICEARCHITECTUREOVERVIEW.md) · [ONBOARDING.md](ONBOARDING.md)
