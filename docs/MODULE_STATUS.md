# 📊 Module Status Matrix

> **Single source of truth for what's built, in progress, and planned.**
> Modules are cataloged in [`BusinessAsUsual.Core/Modules/ModuleCatalog.cs`](../BusinessAsUsual.Core/Modules/ModuleCatalog.cs)
> and surfaced through the Module Registry. This matrix maps the catalog to real
> implementation status.

**Legend:** ✅ Built · 🚧 In Progress · 📐 Scaffolded · 🗓️ Planned

_Totals: **68 modules** across **7 functional categories** + industry-specific packs._

---

## 🧱 Platform (System / Cross-Cutting)

| Module | Status | Notes |
|--------|--------|-------|
| User Management | 🚧 | Users / Roles / Permissions — RBAC core in progress |
| Audit Logs | 🗓️ | System & security events |
| Notifications | 🗓️ | Email / SMS / Push |
| Reporting & Analytics | 🚧 | Dashboards / Exports / KPIs (HR reports built) |
| Integrations | 🗓️ | API Keys / Webhooks / Connectors — see [INTEGRATIONS.md](INTEGRATIONS.md) |
| System Settings | 🗓️ | Company profile / preferences |
| Localization | 🗓️ | Languages / regions |
| **Module Registry** | ✅ | Discovery + UI injection (`services/ModuleRegistry`) |
| **AI Assistant** | 🚧 | Floating UI wired via `IHttpClientFactory` (graceful offline); `services/AI/AI.Api` builds & runs on :5300 with placeholder `/api/ai/embeddings/*` — RAG/LLM pending |

## 💰 Financial

| Module | Status | Notes |
|--------|--------|-------|
| Accounting | 🗓️ | Chart of accounts / journal entries |
| General Ledger | 🗓️ | Ledgers / reconciliation |
| Accounts Receivable | 🗓️ | Invoices / collections |
| Accounts Payable | 🗓️ | Bills / vendor payments |
| Billing | 🗓️ | Recurring / one-time |
| Invoicing | 🗓️ | Templates / delivery |
| Payments | 🗓️ | Gateways / reconciliation |
| Payroll | 🗓️ | Pay runs / deductions |
| Banking | 🗓️ | Accounts / transactions |
| Budgeting | 🗓️ | Forecasts / allocations |
| Taxation | 🗓️ | Rates / filings |

## 🤝 Sales & CRM

| Module | Status | Notes |
|--------|--------|-------|
| **CRM** | ✅ | Leads / Opportunities — `services/CRM` (Domain/App/API/Web) |
| Customers | 🚧 | Profiles / history (part of CRM rollout) |
| Quotes & Estimates | 🗓️ | Drafts / approvals |
| Orders | 🗓️ | Sales orders / fulfillment |
| Subscriptions | 🗓️ | Plans / renewals |
| Point of Sale | 🗓️ | Registers / receipts |
| Products | 🗓️ | Catalog / variants |
| Menu Management | 🗓️ | Items / categories |
| Customer Portal | 🗓️ | Access / self-service |

_See [CRM_FEATURE_ROADMAP.md](CRM_FEATURE_ROADMAP.md) for the CRM feature plan._

## ⚙️ Operations

| Module | Status | Notes |
|--------|--------|-------|
| Inventory | 🗓️ | Stock levels / adjustments |
| Warehousing | 🗓️ | Bins / transfers |
| Purchasing | 🗓️ | Purchase orders / receipts |
| Procurement | 🗓️ | Vendors / contracts |
| Suppliers | 🗓️ | Profiles / ratings |
| Equipment | 🗓️ | Assets / maintenance |
| Maintenance | 🗓️ | Schedules / work orders |
| Vehicles | 🗓️ | Fleet / maintenance |
| Fleet Management | 🗓️ | Dispatch / tracking |
| Logistics | 🗓️ | Shipments / carriers |
| Routing | 🗓️ | Routes / optimization |
| Scheduling | 🗓️ | Calendar / assignments |
| Projects | 🗓️ | Phases / milestones |
| Tasks | 🗓️ | Boards / assignments |
| Work Orders | 🗓️ | Dispatch / completion |
| Services | 🗓️ | Catalog / pricing |
| Workflows | 🗓️ | Automation / triggers |
| Replenishment | 🗓️ | Rules / forecasts |
| Forecasting | 🗓️ | Demand / supply |
| Quality Control | 🗓️ | Inspections / non-conformance |
| Compliance | 🗓️ | Policies / audits |
| Asset Management | 🗓️ | Tracking / depreciation |

## 👥 HR & People

| Module | Status | Notes |
|--------|--------|-------|
| **Human Resources** | ✅ | Employee records / benefits — `services/HR` (full stack) |
| Staff Management | 🚧 | Profiles / roles |
| Recruiting | 🗓️ | Applicants / interviews |
| Onboarding | 🗓️ | Checklists / training |
| Performance | 🗓️ | Reviews / goals |
| Training | 🗓️ | Courses / certifications |
| Timekeeping | 🗓️ | Timesheets / approvals |

## 📄 Documents & Communication

| Module | Status | Notes |
|--------|--------|-------|
| Document Management | 🗓️ | Storage / sharing |
| Messaging | 🗓️ | Conversations / channels |
| Knowledge Base | 🗓️ | Articles / categories |
| File Storage | 🗓️ | Uploads / folders |

## 🏥 Industry-Specific Packs

| Module | Category | Status |
|--------|----------|--------|
| Patients | Healthcare | 🗓️ |
| Clinical Notes | Healthcare | 🗓️ |
| Reservations | Hospitality | 🗓️ |
| Events | Hospitality | 🗓️ |
| Safety | Mining | 🗓️ |
| Dispatch | Logistics | 🗓️ |
| Contracts | Professional Services | 🗓️ |
| Field Service | Professional Services | 🗓️ |

---

## 🎯 Summary

| Status | Count | Modules |
|--------|-------|---------|
| ✅ Built | 3 | HR, CRM, Module Registry |
| 🚧 In Progress | 5 | User Management, Reporting, AI Assistant, Customers, Staff |
| 🗓️ Planned | 60 | Remainder of catalog |

**Solution projects backing the built modules:**
- `services/HR/*` — Domain, Application, API, Infrastructure, Web
- `services/CRM/*` — Domain, Application, API, Web
- `services/ModuleRegistry/*` — Domain, Application, API, Infrastructure
- `backend/BusinessAsUsual.*` — Domain, Application, API, Infrastructure
- `frontend/BusinessAsUsual.Web` + `BusinessAsUsual.Admin`

> Update this matrix whenever a module changes status. It is referenced from
> [INDEX.md](INDEX.md), [CloudProud.md](CloudProud.md), and [README.md](README.md).
