# 🔌 Port Registry

> **Authoritative service → port map for local development.**
> Ports are sourced directly from each project's `Properties/launchSettings.json`.
> When you add a service, reserve a port here first to avoid collisions.

_Last verified against `launchSettings.json` files in the solution._

---

## 🟢 Active Services

### Platform Services
| Service | Project | HTTP URL | HTTPS URL | Layer |
|---------|---------|----------|-----------|-------|
| **Platform Shell (Web)** | `frontend/BusinessAsUsual.Web` | http://localhost:5269 | https://localhost:7229 | UI (Blazor) |
| **Admin Portal** | `frontend/BusinessAsUsual.Admin` | http://localhost:5145 | https://localhost:7238 | UI (Blazor) |
| **Platform API** | `backend/BusinessAsUsual.API` | http://localhost:5000 | https://localhost:5001 | API |
| **Platform Core** | `BusinessAsUsual.Core` | http://localhost:5078 | https://localhost:7139 | Core/Host |
| **Module Registry API** | `services/ModuleRegistry/ModuleRegistry.API` | http://localhost:5100 | https://localhost:7100 | Service API |

### Business Modules
| Module | API Project | API Port (HTTP) | API Port (HTTPS) | Web UI Project | Web UI Port |
|--------|-------------|-----------------|------------------|----------------|-------------|
| **HR** | `services/HR/HR.API` | 5041 | 7171 | `services/HR/HR.Web` | 5002 (http), 7002 (https) |
| **Finance** | `services/Finance/Finance.API` | 5007 | — | `services/Finance/Finance.Web` | 5008 |
| **Inventory** | `services/Inventory/Inventory.API` | 5142 | 7079 | `services/Inventory/Inventory.Web` | 5009 |
| **Sales** | `services/Sales/Sales.API` | 5143 | 7143 | `services/Sales/Sales.Web` | 5293 (http), 7283 (https) |
| **CRM** | `services/CRM/CRM.API` | 5004 | — | `services/CRM/CRM.Web` | 5005 |
| **Services** | `services/Services/Services.API` | 7286 | 7285 | `services/Services/Services.Web` | 61172 (http), 61171 (https) |
| **Platform** | `services/Platform/Platform.API` | 7400 | 7401 | `services/Platform/Platform.Web` | 7402 (http), 7403 (https) |

### Supporting Services
| Service | Project | HTTP URL | HTTPS URL | Notes |
|---------|---------|----------|-----------|-------|
| **AI Microservice** | `services/AI/AI.Api` | http://localhost:5300 | — | `/api/ai/*` |

## 🔴 Shell HttpClient Configuration Status

**Location:** `frontend/BusinessAsUsual.Web/Program.cs`

| Named Client | Configured Base URL | Actual API Port | Status |
|--------------|---------------------|-----------------|--------|
| `HrApi` | http://localhost:5041 | 5041 | ✅ Correct |
| `FinanceApi` | http://localhost:5007 | 5007 | ✅ Correct |
| `InventoryApi` | http://localhost:5142 | 5142 | ✅ Correct |
| `SalesApi` | http://localhost:5143 | 5143 | ✅ Correct |
| `CrmApi` | http://localhost:5004 | 5004 | ✅ Correct |
| `ServicesApi` | http://localhost:7286 | 7286 | ✅ Correct |
| `PlatformApi` | http://localhost:7400 | 7400 | ✅ Configured |
| `AiApi` | http://localhost:5300 | 5300 | ✅ Correct |

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
| `70xx-73xx` | 7000–7399 | Business module APIs (HTTP/HTTPS) |
| `74xx` | 7400–7499 | **Platform module APIs (HTTP/HTTPS)** — Reserved for system-level modules |

> ⚠️ Some existing ports predate this convention (e.g., HR Web `5002`, HR API `5041`, Services API `7286`).
> They are kept as-is to avoid breaking existing launch profiles; new services
> should follow the banding above.

---

## ⚙️ Module Integration Checklist

When adding a new module, ensure:

1. ✅ **API Port Reserved** - Add to PORT_REGISTRY.md before creating launchSettings
2. ✅ **launchSettings.json** - Set applicationUrl in both API and Web projects
3. ✅ **Shell HttpClient Registration** - Add named client in `frontend/BusinessAsUsual.Web/Program.cs`
4. ✅ **Module Discovery** - Add to `ModuleDiscoveryService.cs` fallback list
5. ✅ **Shell Assembly Reference** - Add module Web assembly to `App.razor` AdditionalAssemblies
6. ✅ **MainLayout Route** - Add module route to `MainLayout.razor.cs` UpdateModuleFromUri method (~line 192)
   ```csharp
   else if (path.StartsWith("/yourmodule"))
       _currentModule = "YourModule";
   ```
7. ✅ **Route Prefix** - Use lowercase explicit route in API controllers: `[Route("api/modulename")]`

**⚠️ Common Mistake:** Forgetting step #6 causes the sidebar to be hidden when navigating to module pages!

---

## 🧪 Quick Verify

Re-scan all launch profiles at any time:

```powershell
Get-ChildItem -Recurse -Filter launchSettings.json |
  Select-String -Pattern '"applicationUrl"' |
  Format-Table -AutoSize
```

See also: [MICROSERVICEARCHITECTUREOVERVIEW.md](MICROSERVICEARCHITECTUREOVERVIEW.md) · [ONBOARDING.md](ONBOARDING.md)
