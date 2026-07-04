# CloudProud — Business As Usual Roadmap

This document is a prioritized, actionable roadmap to evolve Business As Usual into a competitive, enterprise‑grade platform. It groups work into phases, lists concrete tasks, deliverables, and acceptance criteria, and includes UI/API contract guidance and a suggested timeline.

## High-level phases (priority order)
1. Foundation & Persistence
2. Security, Identity & Multi‑tenant
3. Core Product Maturity
4. Integrations, Automation & Analytics
5. Mobile, Marketplace & Ecosystem
6. Enterprise Readiness, Scale & Go‑to‑Market

---

## Phase 1 — Foundation & Persistence (0–3 months)
- Migrate mocks → EF Core 9.0
  - Deliverables: DbContext per module, migrations, seeders using existing mock data.
  - Acceptance: All CRUD pages use EF Core-backed services; CI runs migrations.
- Centralize contracts
  - Deliverables: BusinessAsUsual.Contracts package (versioned), used by Web/API/mobile clients.
  - Acceptance: Clients compile against package and generated clients work.
- API surface & versioning
  - Deliverables: OpenAPI v1 for each module; swagger + contract tests in CI.
  - Acceptance: Clients can be generated from OpenAPI; CI validates spec.

## Phase 2 — Security, Identity & Multi‑tenant (1–4 months, overlapping)
- Authentication & authorization
  - Deliverables: Azure AD / OIDC integration, RBAC/claims mapping.
  - Acceptance: SSO login works; role checks enforced in APIs.
- Tenant model & isolation
  - Deliverables: Tenant-aware DbContext and tenant admin UI.
  - Acceptance: Data isolation between two tenants demonstrated.
- Audit, security basics
  - Deliverables: Audit logging, OWASP mitigations, threat model.
  - Acceptance: Security checklist documented and implemented.

## Phase 3 — Core Product Maturity (3–9 months)
- Feature depth per module (CRM/HR/Finance)
  - Deliverables: Workflows (lead assignment, scoring), imports/exports, background jobs.
  - Acceptance: End-to-end user stories covered by tests.
- UI design system & component library
  - Deliverables: MudBlazor theme tokens and reusable Blazor components.
  - Acceptance: All modules use the shared component library.
- API stability & contract governance
  - Deliverables: Versioned OpenAPI specs, contract tests, client SDK generation.
  - Acceptance: Breaking changes require major version bump and CI failure.

## Phase 4 — Integrations, Automation & Analytics (6–12 months)
- Integration layer & connectors
  - Deliverables: Webhooks, sample Slack/Teams/Stripe connectors.
  - Acceptance: Webhook on lead conversion and an inbound connector demonstrated.
- Automation / rules engine
  - Deliverables: Visual rule builder + runtime (or Durable Functions based engine).
  - Acceptance: Create/execute a sample automation rule.
- Analytics & reporting
  - Deliverables: Event capture (OpenTelemetry/events), reporting DB, embedded dashboards.
  - Acceptance: Reports reflect real data and are exportable.

## Phase 5 — Native Mobile & Offline (6–18 months)
- Mobile stack (recommendation: .NET MAUI)
  - Deliverables: MAUI prototype (iOS/Android) using Contracts package, OIDC PKCE auth.
  - Acceptance: Login, view/create leads, offline create + sync without duplication.
- Mobile features
  - Deliverables: Offline store (SQLite), push notifications (FCM/APNs), sync queue.

## Phase 6 — Marketplace, Ecosystem & Enterprise (9–24 months)
- Developer experience & SDKs
  - Deliverables: C#/.NET and TypeScript SDKs, developer portal, sample modules.
- Marketplace & third-party modules
  - Deliverables: Plugin contract for UI+API registration, sample partner module.
- Production operations
  - Deliverables: Docker images, Helm charts, CI/CD pipelines, observability (OpenTelemetry + Grafana), backup/restore runbooks.

---

## UI & API Contracts (concrete)

API patterns (OpenAPI v1)
- Base path: /api/v1/{module}/{resource}
- Pagination: ?page=1&pageSize=50
- Filtering: ?status=Qualified&source=Website
- Sorting: ?sort=-createdAt
- Auth: Bearer tokens + scopes (e.g., crm.leads.read)

Example CRM endpoints
- GET /api/v1/crm/leads
- GET /api/v1/crm/leads/{id}
- POST /api/v1/crm/leads
- PUT /api/v1/crm/leads/{id}
- DELETE /api/v1/crm/leads/{id}
- POST /api/v1/crm/leads/{id}/convert

Blazor route contract (shell)
- /crm (dashboard)
- /crm/leads, /crm/leads/new, /crm/leads/create, /crm/leads/{id}
- /crm/opportunities, /crm/opportunities/new, /crm/opportunities/{id}
- /crm/customers, /crm/customers/new, /crm/customers/{id}

Component contracts (recommended)
- DataGrid<T>: Items IEnumerable<T>, OnRowClick(T), OnDelete(T)
- Form<T>: Model T, OnValidSubmit(Model), ValidationSummary
- Global services: Snackbar, Error Handler, Loading overlay

---

## Suggested timeline & milestones (example)
- Sprint 0 (2 weeks): Contracts package, OpenAPI skeleton, EF Core scaffolding, CI build
- Sprints 1–4 (8 weeks): EF Core migrations + seeders, wire APIs to DB, basic auth
- Sprints 5–12 (12 weeks): Feature parity for CRM/HR, UI component library
- Months 4–9: Integrations, automation, analytics, MAUI mobile MVP
- Months 9–18: Marketplace, enterprise polish, observability & DR

## Quick wins (first 30 days)
- Replace mock services with EF Core + seed data
- Publish OpenAPI for CRM and generate C#/TypeScript client
- Add Identity/SSO skeleton (Azure AD) and protect APIs
- Build a shared Blazor component library and migrate Dashboard/Leads to it
- Create a MAUI prototype that uses the generated contracts package

## Next actions (pick one to start)
1. Generate OpenAPI spec skeleton for CRM endpoints (I can scaffold this)
2. Scaffold EF Core DbContext + initial migrations for CRM (I can add the DbContext and seeders)
3. Scaffold a .NET MAUI mobile prototype (basic auth + leads list)

---

Notes: This roadmap assumes .NET 9 and Blazor as the primary web UI technology. Prioritize contract-first APIs and shared contracts for fast client generation (web + mobile). Adjust timelines based on team size and availability of cloud infra for testing.
