# Business As Usual — Microservice Architecture Overview

```mermaid
flowchart TB

    %% Core Star System
    subgraph Core["Core Star System"]
        A[Admin App]
        B[Main App]
        C[BAU API]
        D[Auth Service]
        E[Provisioning Engine]
        F[Metrics Engine]
        G[Logging Pipeline]
        H[Settings Manager]
        I[Theme Engine]
    end

    %% Frontend Cluster
    subgraph Frontend["Frontend Cluster"]
        J[MVC/Blazor Frontends]
        K[Universal Theme Layer]
        L[Runtime Interaction Layer]
    end

    %% Infrastructure Nebula
    subgraph Infra["Infrastructure Nebula"]
        M[Nginx Reverse Proxy]
        N[EC2 Compute]
        O[RDS Microsoft SQL]
        P[S3 Static Assets]
        Q[CloudFront CDN]
        R[EC2 Log Storage]
    end

    %% Deployment Constellation
    subgraph Deploy["Deployment Constellation"]
        S[GitHub Actions]
        T[S3 Artifact Bucket]
        U[CodeDeploy]
    end

    %% Future Systems
    subgraph Future["Outer Systems"]
        V[BAU Mobile - Android]
        W[BAU Mobile - iOS]
        X[Push Notifications]
        Y[Offline Sync]
        Z[BAU Desktop]
    end

    %% Connections
    J --> A
    J --> B
    A --> C
    B --> C
    C --> D
    C --> E
    C --> F
    C --> H
    C --> G

    A --> I
    B --> I
    I --> K
    K --> L

    M --> A
    M --> B
    M --> C

    C --> O
    A --> P
    B --> P
    P --> Q

    G --> R

    S --> T
    T --> U
    U --> N

    V --> C
    W --> C
    X --> V
    X --> W
    Y --> C
    Z --> C
```

---

<details>
<summary><strong>📘 Table of Contents</strong></summary>

---

## 🧭 Overview & Purpose
- [Purpose of This Document](#purpose-of-this-document)
- [Why BAU Uses a Microservice Architecture](#why-bau-uses-a-microservice-architecture)
- [What “Module = Microservice” Means](#what-module--microservice-means)
- [How Services Communicate](#how-services-communicate)
  - [1. Synchronous API Calls](#1-synchronous-api-calls)
  - [2. Asynchronous Events](#2-asynchronous-events)
- [How the UI Discovers Modules](#how-the-ui-discovers-modules)
- [Tenant Isolation Strategy](#tenant-isolation-strategy)
- [How Modules Handle Cross‑Domain Data](#how-modules-handle-crossdomain-data)

---

## 🏛️ Module Registry Service (MRS)
- [Purpose](#purpose)
- [Responsibilities](#responsibilities)
- [Module Metadata Model](#module-metadata-model)
- [Module Registration Flow](#module-registration-flow)
- [Discovery Endpoints](#discovery-endpoints)
- [Health Checking](#health-checking)
- [Multi‑Tenant Routing](#multitenant-routing)
- [Security](#security)
- [Deployment Model](#deployment-model)
- [Why MRS Matters](#why-mrs-matters)

---

## 🚪 API Gateway & Routing Strategy
- [Purpose](#purpose-1)
- [Responsibilities](#responsibilities-1)
- [Routing Model](#routing-model)
- [Tenant Resolution](#tenant-resolution)
- [Authentication & Authorization](#authentication--authorization)
- [Request Aggregation](#request-aggregation-optional)
- [Health & Diagnostics](#health--diagnostics)
- [Deployment Model](#deployment-model-1)
- [Why the Gateway Matters](#why-the-gateway-matters)

---

## 🧩 BAU Microservice Template
- [Purpose](#purpose-2)
- [Folder Structure](#folder-structure)
- [Layer Responsibilities](#layer-responsibilities)
- [Required Endpoints](#required-endpoints)
- [Health Checks](#health-checks)
- [Logging Standards](#logging-standards)
- [Database Rules](#database-rules)
- [Event Publishing](#event-publishing)
- [Event Subscribing](#event-subscribing)
- [Module Registration](#module-registration)
- [Deployment Expectations](#deployment-expectations)
- [Example Minimal Module](#example-minimal-module)

---

## ⏱️ Example Microservice: Timekeeping
- [Purpose](#purpose-3)
- [Domain Overview](#domain-overview)
- [Folder Structure](#folder-structure-1)
- [Data Model](#data-model-simplified)
- [API Surface](#api-surface)
- [Tenant Handling](#tenant-handling)
- [Integration with Other Modules](#integration-with-other-modules)
- [Events](#events)
- [UI Integration](#ui-integration)
- [Health & Diagnostics](#health--diagnostics-1)
- [Deployment](#deployment)
- [Summary](#summary-1)

---

## 🗄️ Database Strategy — Tenant‑Per‑Database
- [Purpose](#purpose-4)
- [Core Principles](#core-principles)
- [Database Naming Convention](#database-naming-convention)
- [Migrations](#migrations)
- [Cross‑Module Data Access](#crossmodule-data-access)
- [Local Read Models](#local-read-models)
- [Backup & Restore](#backup--restore)
- [Scaling Strategy](#scaling-strategy)
- [Why This Strategy Works](#why-this-strategy-works)

---

## 🔔 Event‑Driven Architecture
- [Purpose](#purpose-5)
- [Core Concepts](#core-concepts)
- [Architecture Overview](#architecture-overview)
- [Event Flow](#event-flow)
- [Example: Business Provisioning](#example-business-provisioning)
- [Why This Matters](#why-this-matters)

---

## 📱 Impact on Mobile Apps
- [Key Advantages](#key-advantages)
- [Summary](#summary-2)

---

## 🖼️ Diagrams
- [Diagram 1 — High‑Level Architecture](#-diagram-1---high-level-architecture-diagram)
- [Diagram 2 — Deployment Pipeline](#-diagram-2---deployment-pipeline-github---aws---ec2)
- [Diagram 3 — Network Topology](#️-diagram-3---network-topology-vpc-subnets-routing)
- [Diagram 4 — Service Dependency Graph](#-diagram-4---service-dependency-graph)
- [Diagram 5 — Database Schema](#️-diagram-5---database-schema-erd-style-mermaid)
- [Diagram 6 — User Login Sequence](#-diagram-6---user-login-sequence-flow)
- [Diagram 7 — Tech Stack Grid](#-diagram-7---tech-stack-grid)
- [Diagram 8 — Nginx Reverse Proxy Routing](#-diagram-8---nginx-reverse-proxy-routing)
- [Diagram 9 — System Metric Pipeline](#-diagram-9---system-metric-pipeline-collectors---api---dashboard)
- [Diagram 10 — Provisioning Wizard Flow](#-diagram-10---provisioning-wizard-flow-business-creation--module-setup)
- [Diagram 11 — API Routing Map](#️-diagram-11---api-routing-map-domain-organized-endpoints)
- [Diagram 12 — Theme Engine Architecture](#-diagram-12---multi-tenant-theme-engine-architecture)
- [Diagram 13 — Admin Dashboard Layout](#-diagram-13---admin-dashboard-layout-architecture)
- [Diagram 14 — Logging & Diagnostics Pipeline](#-diagram-14---logging--diagnostics-pipeline)
- [Diagram 15 — Configuration & Settings Flow](#️-diagram-15---configuration--settings-flow)
- [Diagram 16 — Authentication Token Lifecycle](#-diagram-16---authentication-token-lifecycle)
- [Diagram 17 — Static Assets Delivery Pipeline](#-diagram-17---static-assets-delivery-pipeline)
- [Diagram 18 — Full Deployment Pipeline](#-diagram-18---full-deployment-pipeline-github---aws---ec2---nginx)
- [Diagram 19 — Error Handling & Recovery Flow](#️-diagram-19---error-handling--recovery-flow)
- [Diagram 20 — Module & Submodule Hierarchy](#-diagram-20---module--submodule-heirarchy-erp-structure)
- [Diagram 21 — Full System Overview](#-diagram-21---full-system-overview-all-components)
- [Diagram 22 — Future Mobile Architecture](#-diagram-22---future-mobile-architecture-android--ios)

</details>

---

## Purpose of This Document
This document provides a complete, contributor‑ready overview of the Business As Usual (BAU) microservice architecture. It explains how modules are structured, how services communicate, how data flows between domains, and how the platform achieves scalability, resilience, and tenant isolation.

## Why BAU Uses a Microservice Architecture
BAU is designed to support many business types, each with unique operational needs. A monolithic architecture would become rigid, slow to evolve, and difficult for contributors to extend. Microservices allow BAU to:

- Scale individual modules independently  
- Deploy updates without affecting unrelated features  
- Maintain clear domain boundaries  
- Support tenant‑per‑database isolation  
- Allow contributors to build new modules without touching the core system  

Each module is a self‑contained service with its own API, database, and deployment lifecycle.

## What “Module = Microservice” Means
In BAU, a *module* is a fully independent microservice. Each module:

- Owns its own data  
- Exposes a REST API  
- Registers itself with the Module Registry Service (MRS)  
- Publishes and consumes domain events  
- Provides optional UI components that the Admin/Main apps can load dynamically  

Modules do not share databases. Modules communicate through APIs and events.

## How Services Communicate
BAU uses two communication patterns:

### 1. Synchronous API Calls
Modules expose REST endpoints for real‑time queries and commands.  
Example: The Orders module may call the Products module to retrieve product details.

### 2. Asynchronous Events
Modules publish events when their data changes.  
Other modules subscribe and maintain local read‑optimized copies of the data they need.

This avoids cross‑database joins and keeps modules decoupled.

## How the UI Discovers Modules
The Admin and Main applications do not hard‑code module lists.  
Instead, they query the Module Registry Service (MRS), which returns:

- Available modules  
- Module metadata  
- API base URLs  
- UI component URLs  
- Permissions and capabilities  

This allows modules to be added or removed without modifying the core UI.

## Tenant Isolation Strategy
BAU uses a **tenant‑per‑database** model.  
Each module connects to the tenant’s database using credentials resolved at runtime.

This ensures:

- Strong data isolation  
- Easy data export/import  
- Per‑tenant scaling  
- Clean separation of concerns  

Modules never share databases, even within the same tenant.

## How Modules Handle Cross‑Domain Data
Modules never join across databases.  
Instead, they compose data through:

- API calls  
- Event‑driven local read models  
- Hybrid caching strategies  

This ensures performance, resilience, and clean domain boundaries.

---

# Module Registry Service (MRS)

## Purpose
The Module Registry Service (MRS) is the central directory for all BAU modules. It allows the Admin and Main applications to dynamically discover available modules, retrieve metadata, load UI components, and route API calls without hard‑coding any module information.

MRS enables BAU to remain modular, extensible, and plug‑and‑play. New modules can be added, updated, or removed without modifying the core applications.

## Responsibilities
The MRS is responsible for:

- Registering modules when they come online  
- Storing module metadata  
- Providing module discovery endpoints  
- Validating module health  
- Exposing API base URLs for routing  
- Exposing UI component URLs for dynamic loading  
- Managing module permissions and capabilities  
- Supporting multi‑tenant routing rules  

The MRS does **not** store business data. It only stores module metadata.

## Module Metadata Model
Each module registers itself with the following metadata:

- `moduleId` — Unique identifier (e.g., "orders")  
- `displayName` — Human‑readable name  
- `description` — Short summary of the module  
- `version` — Semantic version of the module  
- `apiBaseUrl` — Base URL for the module’s API  
- `uiEntryPoint` — URL to the module’s UI bundle (optional)  
- `permissions` — List of permissions the module exposes  
- `capabilities` — Feature flags or module‑specific capabilities  
- `healthUrl` — Endpoint used for health checks  
- `tenantMode` — How the module handles tenant isolation (e.g., tenant‑per‑db)  

This metadata is stored in the MRS database and returned to clients on request.

## Module Registration Flow
1. A module starts up.  
2. It sends a `POST /modules/register` request to the MRS.  
3. The MRS validates the payload.  
4. The MRS stores or updates the module metadata.  
5. The MRS begins periodic health checks.  
6. The module becomes discoverable by Admin/Main apps.

Modules may re‑register at any time (e.g., after deployment).

## Discovery Endpoints

### `GET /modules`
Returns a list of all registered modules with their metadata.

### `GET /modules/{moduleId}`
Returns metadata for a specific module.

### `GET /modules/active`
Returns only modules that are currently healthy and available.

### `GET /modules/ui`
Returns a list of modules that expose UI components.

### `GET /modules/capabilities`
Returns a flattened list of all module capabilities.

These endpoints allow the Admin/Main apps to dynamically build navigation, load UI bundles, and route API calls.

## Health Checking
The MRS performs periodic health checks using each module’s `healthUrl`.

A module is considered **healthy** if:

- The health endpoint returns HTTP 200  
- The module responds within the timeout window  
- The module’s version matches the expected schema version (optional rule)  

Unhealthy modules are excluded from the `/modules/active` list.

## Multi‑Tenant Routing
The MRS does not store tenant data.  
Instead, it provides routing metadata that allows clients to:

- Resolve the correct API base URL  
- Determine whether a module uses tenant‑per‑database  
- Pass tenant identifiers to the module  

Each module is responsible for connecting to the correct tenant database.

## Security
The MRS enforces:

- API key or JWT authentication for module registration  
- Role‑based access for Admin/Main discovery endpoints  
- Validation of module metadata  
- Optional signature validation for UI bundles  

The MRS does not handle user authentication; it only manages module metadata.

## Deployment Model
The MRS is deployed as a standalone microservice with:

- Its own database  
- Its own API  
- Its own health checks  
- Its own CI/CD pipeline  

It must be online before any other module can register.

## Why MRS Matters
Without the MRS:

- The Admin/Main apps would require hard‑coded module lists  
- Adding new modules would require code changes  
- Modules could not be independently deployed  
- UI composition would break  
- Routing would be brittle and error‑prone  

The MRS is the backbone of BAU’s modular architecture.

---

# API Gateway & Routing Strategy

## Purpose
The API Gateway is the single entry point for all client requests in the BAU ecosystem. It centralizes routing, authentication, authorization, tenant resolution, and request forwarding to individual modules. This ensures that modules remain simple, independent, and focused on their domain logic.

The gateway allows BAU to maintain a clean, scalable, and secure microservice architecture.

## Responsibilities
The API Gateway handles:

- Routing requests to the correct module  
- Injecting tenant context into requests  
- Validating authentication tokens  
- Enforcing authorization rules  
- Rate limiting and throttling  
- Logging and request tracing  
- Aggregating responses when needed  
- Providing a stable public API surface  

Modules do not expose their APIs directly to clients. All traffic flows through the gateway.

## Routing Model
The gateway uses module metadata from the Module Registry Service (MRS) to determine routing rules.

### Example Routing Pattern

```
/api/{moduleId}/{resource}/{id?}
```

Examples:
- `/api/orders/create`
- `/api/products/123`
- `/api/customers/search`

The gateway resolves `{moduleId}` using the MRS and forwards the request to the module’s `apiBaseUrl`.

## Tenant Resolution
The gateway is responsible for determining the active tenant for each request.

Tenant resolution may use:

- JWT claims  
- Subdomain (e.g., `tenant1.businessasusual.work`)  
- Query parameters (fallback)  
- Admin‑selected tenant context  

Once resolved, the gateway injects tenant metadata into the forwarded request:

- `X-Tenant-Id`  
- `X-Tenant-Database` (optional)  
- `X-Tenant-User`  

Modules use this metadata to connect to the correct tenant database.

## Authentication & Authorization
The gateway validates:

- Access tokens  
- Refresh tokens  
- Role claims  
- Module‑specific permissions  

Authorization rules are derived from:

- User roles  
- Module permissions (from MRS)  
- Capability flags  

Unauthorized requests never reach the modules.

## Request Aggregation (Optional)
Some UI screens require data from multiple modules.  
The gateway may provide aggregation endpoints to reduce client round‑trips.

Example:

```
GET /api/dashboard/overview
```

This endpoint may internally call:

- Orders module  
- Products module  
- Customers module  

Aggregation is used sparingly to avoid coupling.

## Health & Diagnostics
The gateway exposes:

- `/health` — basic health  
- `/metrics` — performance metrics  
- `/routes` — active routing table  
- `/modules` — cached module metadata  

This helps contributors debug routing issues.

## Deployment Model
The gateway is deployed as a standalone service with:

- Its own CI/CD pipeline  
- Horizontal scaling  
- Load balancer integration  
- Caching layer for module metadata  

It must be online for the platform to function.

## Why the Gateway Matters
Without the gateway:

- Clients would need to know every module’s URL  
- Authentication would be duplicated across modules  
- Tenant resolution would be inconsistent  
- Modules would be exposed directly to the internet  
- Routing changes would break clients  

The gateway provides stability, security, and simplicity across the entire BAU ecosystem.

---

# BAU Microservice Template

## Purpose
This template defines the required structure, conventions, and behaviors for all BAU microservices (modules). Every module must follow this standard to ensure consistency, maintainability, and seamless integration with the BAU ecosystem.

The template covers:
- Folder structure  
- Required endpoints  
- Health checks  
- Logging  
- Database rules  
- Event publishing/subscribing  
- Module registration  
- Deployment expectations  

This document serves as the blueprint for contributors building new modules.

---

# Folder Structure

```
/src
	/ModuleName.Api
	/ModuleName.Application
	/ModuleName.Domain
	/ModuleName.Infrastructure
	/tests
		/ModuleName.Tests
	/docker
	/scripts
	README.md

```

### Layer Responsibilities

**ModuleName.Api**  
- Controllers  
- Request/response models  
- Authentication/authorization  
- Tenant context extraction  
- API versioning  

**ModuleName.Application**  
- Use cases  
- Command handlers  
- Query handlers  
- Validation  
- DTO mapping  

**ModuleName.Domain**  
- Entities  
- Value objects  
- Domain events  
- Aggregates  
- Business rules  

**ModuleName.Infrastructure**  
- Database context  
- Repositories  
- Event bus integration  
- External service clients  
- Migrations  

---

# Required Endpoints

Every module must expose the following endpoints:

### `GET /health`
Returns module health status.

### `GET /info`
Returns:
- Module ID  
- Version  
- Description  
- Capabilities  
- Build timestamp  

### `POST /register`
Used during startup to register with the Module Registry Service (MRS).

### Domain Endpoints
Each module defines its own domain endpoints following REST conventions:

```
GET    /{resource}
GET    /{resource}/{id}
POST   /{resource}
PUT    /{resource}/{id}
DELETE /{resource}/{id}
```

---

# Health Checks

Each module must implement:

- Database connectivity check  
- External dependency check (if applicable)  
- Event bus connectivity check  
- Internal service check  

Health endpoint must return:

```json
{
  "status": "Healthy",
  "details": {
	"database": "Healthy",
	"eventBus": "Healthy",
	"externalService": "Healthy"
  }
}
```

Unhealthy modules are excluded from `/modules/active` in the MRS.

---

# Logging Standards

Modules must use structured logging with:

- Correlation ID  
- Tenant ID  
- User ID  
- Module ID  
- Request path  
- Execution time  
- Error details  

Log levels:
- `Information` for normal operations  
- `Warning` for recoverable issues  
- `Error` for failures  
- `Critical` for outages  

Logs must be JSON‑formatted for ingestion by centralized logging.

---

# Database Rules

Each module owns its own database schema.

Rules:
- No cross‑module tables  
- No foreign keys to other modules  
- No shared schemas  
- No distributed joins  
- Migrations must be idempotent  
- Tenant‑per‑database connection strings resolved at runtime  

Modules may maintain **local read models** populated via events.

---

# Event Publishing

Modules publish domain events when meaningful changes occur.

Example events:
- `OrderCreated`
- `ProductUpdated`
- `CustomerActivated`

Event payloads must include:
- Event ID  
- Timestamp  
- Module ID  
- Tenant ID  
- Version  
- Payload  

Events must be versioned to avoid breaking consumers.

---

# Event Subscribing

Modules may subscribe to events from other modules.

Rules:
- Subscribers must handle unknown fields gracefully  
- Subscribers must be idempotent  
- Subscribers must not block the event bus  
- Subscribers must update local read models only  
- Subscribers must not perform cross‑module writes  

---

# Module Registration

On startup, each module must call:

```
POST /modules/register
```

Payload includes:
- moduleId  
- displayName  
- description  
- version  
- apiBaseUrl  
- uiEntryPoint (optional)  
- permissions  
- capabilities  
- healthUrl  
- tenantMode  

Modules must re‑register on deployment.

---

# Deployment Expectations

Each module must support:

- Independent deployment  
- Zero‑downtime restarts  
- Rolling updates  
- Health‑based load balancer removal  
- Environment‑based configuration  
- CI/CD pipeline integration  

Modules must not assume:
- Shared memory  
- Shared caches  
- Shared databases  
- Shared configuration files  

---

# Example Minimal Module

```
ModuleName.Api/
	Controllers/
		ResourceController.cs
	Program.cs
	appsettings.json

ModuleName.Application/
	Commands/
	Queries/
	Validators/

ModuleName.Domain/
	Entities/
	Events/
	ValueObjects/

ModuleName.Infrastructure/
	Db/
	Repositories/
	EventBus/
	Migrations/
```

---

# Summary
This template ensures that every BAU module is:

- Independent  
- Consistent  
- Scalable  
- Testable  
- Discoverable  
- Tenant‑aware  
- Event‑driven  

Following this standard guarantees that new modules integrate seamlessly into the BAU ecosystem.

---

# Example Microservice: Timekeeping

## Purpose
The Timekeeping module manages employee time entries, shifts, and approvals. It is a standalone microservice that follows the BAU microservice template and integrates with other modules (e.g., Employees, Projects, Payroll) via APIs and events.

This example demonstrates how a real module is structured, how it exposes its API, how it handles tenant isolation, and how it participates in the event‑driven ecosystem.

---

## Domain Overview

The Timekeeping module is responsible for:

- Recording time entries for employees  
- Associating time with projects, jobs, or cost centers  
- Managing shift rules and schedules  
- Handling approvals (supervisor, manager, payroll)  
- Publishing events for downstream consumers (e.g., Payroll)  

It does **not** own employee master data or payroll rules. Those belong to other modules.

---

## Folder Structure

The Timekeeping module follows the standard BAU microservice template:

```
/src
	/Timekeeping.Api
	/Timekeeping.Application
	/Timekeeping.Domain
	/Timekeeping.Infrastructure
	/tests
		/Timekeeping.Tests
	/docker
	/scripts
	README.md

```

---

## Data Model (Simplified)

### Core Entities

**TimeEntry**
- `Id`  
- `TenantId`  
- `EmployeeId`  
- `ProjectId` (optional)  
- `CostCenterId` (optional)  
- `StartTime`  
- `EndTime`  
- `DurationMinutes`  
- `EntryType` (e.g., Regular, Overtime, PTO)  
- `Status` (Draft, Submitted, Approved, Rejected)  
- `CreatedAt`  
- `ApprovedAt` (optional)  

**Shift**
- `Id`  
- `TenantId`  
- `Name`  
- `StartTime`  
- `EndTime`  
- `IsOvernight`  
- `IsFlexible`  

**Approval**
- `Id`  
- `TenantId`  
- `TimeEntryId`  
- `ApproverId`  
- `Status` (Pending, Approved, Rejected)  
- `Comment`  
- `Timestamp`  

The Timekeeping database is tenant‑per‑database and owned exclusively by this module.

---

## API Surface

Base path (via gateway):

```
/api/timekeeping
```

### Time Entries

**GET `/api/timekeeping/time-entries`**  
Returns a paged list of time entries for the current tenant (and optionally filtered by employee, date range, status).

**GET `/api/timekeeping/time-entries/{id}`**  
Returns a single time entry.

**POST `/api/timekeeping/time-entries`**  
Creates a new time entry.

**PUT `/api/timekeeping/time-entries/{id}`**  
Updates an existing time entry (if not yet approved).

**POST `/api/timekeeping/time-entries/{id}/submit`**  
Submits a time entry for approval.

**POST `/api/timekeeping/time-entries/{id}/approve`**  
Approves a time entry.

**POST `/api/timekeeping/time-entries/{id}/reject`**  
Rejects a time entry.

### Shifts

**GET `/api/timekeeping/shifts`**  
Returns defined shifts.

**POST `/api/timekeeping/shifts`**  
Creates a new shift definition.

### System Endpoints

**GET `/api/timekeeping/health`**  
Health check.

**GET `/api/timekeeping/info`**  
Module metadata (ID, version, capabilities).

---

## Tenant Handling

The Timekeeping module does not determine the tenant itself.  
It receives tenant context from the API Gateway via headers:

- `X-Tenant-Id`  
- `X-Tenant-Database` (optional)  

The Infrastructure layer uses this context to:

- Resolve the correct connection string  
- Connect to the tenant’s Timekeeping database  
- Apply tenant filters to all queries  

No cross‑tenant data is ever mixed.

---

## Integration with Other Modules

### Employees (via API)

The Timekeeping module does **not** store full employee records.  
Instead, it:

- Stores `EmployeeId` as a foreign key (logical, not DB‑level to another module)  
- Calls the Employees module when it needs employee details  

Example call (from Application layer):

- `GET /api/employees/{employeeId}`

### Projects / Jobs (via API or Events)

For project or job context, Timekeeping can:

- Call the Projects module API for real‑time details  
- Or maintain a local read model populated by `ProjectUpdated` events  

### Payroll (via Events)

When time entries are approved, Timekeeping publishes events:

- `TimeEntryApproved`
- `TimeEntryRejected`

The Payroll module subscribes and uses these events to calculate pay.

---

## Events

### Published Events

**TimeEntryCreated**
- `eventId`  
- `timestamp`  
- `moduleId: "timekeeping"`  
- `tenantId`  
- `version`  
- `payload`:
  - `timeEntryId`  
  - `employeeId`  
  - `startTime`  
  - `endTime`  
  - `durationMinutes`  
  - `entryType`  

**TimeEntryApproved**
- `eventId`  
- `timestamp`  
- `moduleId: "timekeeping"`  
- `tenantId`  
- `version`  
- `payload`:
  - `timeEntryId`  
  - `employeeId`  
  - `approvedAt`  
  - `durationMinutes`  
  - `entryType`  

### Subscribed Events (Example)

**EmployeeUpdated** (from Employees module)  
Used to update local read models for employee display names or statuses.

**ProjectUpdated** (from Projects module)  
Used to update local project references for reporting.

---

## UI Integration

The Timekeeping module may expose a UI bundle:

- `uiEntryPoint`: URL to a Timekeeping SPA or micro‑frontend

The Admin/Main apps:

1. Query the MRS for modules with UI components.  
2. Discover Timekeeping’s `uiEntryPoint`.  
3. Dynamically load the Timekeeping UI into the shell (e.g., under “Time & Attendance”).

The Timekeeping UI communicates only with the gateway (`/api/timekeeping/...`), never directly with the module.

---

## Health & Diagnostics

**GET `/api/timekeeping/health`** checks:

- Database connectivity  
- Event bus connectivity  
- External dependencies (if any)  

**GET `/api/timekeeping/info`** returns:

- `moduleId: "timekeeping"`  
- `version`  
- `description`  
- `capabilities` (e.g., `["time-entry", "shift-management", "approvals"]`)  

These endpoints are used by:

- MRS for health status  
- Admin tools for diagnostics  

---

## Deployment

The Timekeeping module:

- Is deployed as an independent service  
- Has its own CI/CD pipeline  
- Can be scaled horizontally based on load  
- Can be updated without redeploying other modules  

On startup, it calls:

```
POST /modules/register
```

with its metadata so the MRS can make it discoverable.

---

## Summary

The Timekeeping module demonstrates how a BAU microservice:

- Owns its domain and database  
- Integrates with other modules via APIs and events  
- Respects tenant isolation  
- Exposes a clean, RESTful API  
- Publishes events for downstream consumers  
- Optionally provides a UI bundle for dynamic loading  

This example serves as a reference implementation for contributors building new modules.

---

# Database Strategy — Tenant‑Per‑Database

## Purpose
This section defines how Business As Usual (BAU) manages data storage across tenants and modules. The platform uses a strict **tenant‑per‑database** model to ensure isolation, security, scalability, and clean domain boundaries. Each module owns its own schema and connects to the correct tenant database at runtime.

This strategy supports BAU’s multi‑tenant SaaS architecture while keeping modules independent and resilient.

---

# Core Principles

## 1. Tenant‑Per‑Database
Each tenant receives its own database for each module.

Example:
- Tenant A → `bau_tenantA_timekeeping`, `bau_tenantA_orders`, `bau_tenantA_products`
- Tenant B → `bau_tenantB_timekeeping`, `bau_tenantB_orders`, `bau_tenantB_products`

This ensures:
- Strong data isolation  
- Easy export/import  
- Per‑tenant scaling  
- No cross‑tenant contamination  
- Clean auditability  

---

## 2. Module‑Owned Schemas
Each module owns its own schema and tables.

Rules:
- No shared tables  
- No shared schemas  
- No cross‑module foreign keys  
- No distributed joins  
- No module may read another module’s database  

Modules communicate through:
- REST APIs  
- Events  
- Local read models  

---

## 3. Runtime Connection Resolution
Modules do not store connection strings in configuration files.

Instead:
1. The API Gateway resolves the tenant.  
2. The gateway injects tenant metadata into headers:  
   - `X-Tenant-Id`  
   - `X-Tenant-Database` (optional)  
3. The module’s Infrastructure layer uses this metadata to build the correct connection string.  

This allows:
- One deployed module instance to serve all tenants  
- Zero configuration changes when adding tenants  
- Clean separation between routing and storage  

---

# Database Naming Convention

Each module uses the following naming pattern:

```
bau_{tenantId}_{moduleId}
```

Examples:
- `bau_acme_timekeeping`
- `bau_acme_orders`
- `bau_globex_products`

This ensures predictable, discoverable database names.

---

# Migrations

Each module maintains its own migrations.

Rules:
- Migrations must be idempotent  
- Migrations must run per‑tenant  
- Migrations must not reference other modules’ tables  
- Migrations must be versioned and tracked  

Deployment pipelines may:
- Run migrations automatically  
- Or run migrations manually per tenant  

---

# Cross‑Module Data Access

Modules never join across databases.

Instead, they use:

### 1. API Calls  
For real‑time, fresh data.  
Example: Orders → Products for product details.

### 2. Event‑Driven Read Models  
For fast, resilient reads.  
Example: Orders subscribes to `ProductUpdated` and stores a local copy.

### 3. Hybrid  
Most modules use both.

This avoids:
- Distributed transactions  
- Cross‑service coupling  
- Schema dependencies  

---

# Local Read Models

A local read model is a small, denormalized table that stores only the fields a module needs from another domain.

Example (Orders module):

```
ProductReadModel
- ProductId
- Name
- SKU
- Price
- LastUpdated
```

Populated by:
- `ProductCreated`
- `ProductUpdated`
- `ProductDeleted`

Benefits:
- Fast queries  
- No dependency on other modules’ uptime  
- No cross‑database joins  

---

# Backup & Restore

Because each tenant has isolated databases:

- Backups can be tenant‑specific  
- Restores do not affect other tenants  
- Data exports are simple  
- Compliance (HIPAA, SOC2, etc.) is easier  

Modules may define:
- Backup frequency  
- Retention policies  
- Encryption requirements  

---

# Scaling Strategy

Tenant‑per‑database enables flexible scaling:

### Horizontal Scaling  
Add more module instances to handle more tenants.

### Vertical Scaling  
Increase compute/storage for high‑usage tenants.

### Sharding (Future)  
Large tenants can be split across multiple databases.

---

# Why This Strategy Works

Tenant‑per‑database provides:

- Maximum isolation  
- Maximum flexibility  
- Clean domain boundaries  
- Predictable performance  
- Simple compliance  
- Easy onboarding/offboarding  
- Zero cross‑tenant risk  

This is the same strategy used by:
- Shopify  
- Salesforce  
- Many enterprise SaaS platforms  

It is proven, scalable, and future‑proof.

---

# Event‑Driven Architecture
A resilient, decoupled backbone for operational automation.

## Purpose
The platform uses an event‑driven model to keep operational workflows asynchronous, scalable, and fault‑tolerant. Instead of chaining synchronous API calls, services emit domain events that downstream consumers react to independently. This prevents bottlenecks, isolates failures, and allows new capabilities to be added without modifying existing services.

## Core Concepts

### Domain Events
Events represent meaningful business actions such as:

- UserCreated
- BusinessProvisioned
- ModuleEnabled
- InvoiceGenerated
- HealthCheckFailed

Events are immutable and describe what happened, not what should happen next.

### Event Producers
Any service can publish events, including:

- Admin API (provisioning workflows)
- Main API (user actions, module activity)
- Background workers (scheduled tasks, health monitors)

### Event Consumers
Consumers subscribe to specific event types and perform follow‑up actions:

- Provisioning workers
- Notification services
- Metrics collectors
- Audit log writers
- External integrations

Consumers are isolated, so failures never cascade.

## Architecture Overview

| Component | Role |
|----------|------|
| SNS Topics | Broadcast events to multiple subscribers |
| SQS Queues | Buffer events for reliable, ordered processing |
| Workers / Lambdas | Consume events and perform actions |
| Dead‑Letter Queues | Capture failed events for inspection |

This pattern ensures at‑least‑once delivery, horizontal scalability, and operational transparency.

## Event Flow

1. A service publishes an event to an SNS topic.  
2. SNS fans out the event to one or more SQS queues.  
3. Workers poll their queue and process events independently.  
4. Failures are retried, and persistent failures land in a DLQ.  
5. Metrics and logs capture the full lifecycle for observability.

## Example: Business Provisioning

When a new business is created:

1. Admin API emits `BusinessProvisioned`.  
2. SNS distributes the event.  
3. Workers handle:  
   - Creating default modules  
   - Seeding tenant data  
   - Generating audit entries  
   - Sending onboarding notifications  
4. Each worker runs independently, so slow tasks never block the user experience.

## Why This Matters

Event‑driven architecture provides:

- Resilience through failure isolation  
- Scalability through horizontal worker expansion  
- Extensibility through plug‑and‑play consumers  
- Observability through event logs and metrics  
- A smoother user experience by offloading long‑running tasks

This architecture is the backbone that makes BAU feel fast, modern, and enterprise‑ready.

---

# Impact on Mobile Apps (Android & iOS)

The event‑driven architecture directly benefits the mobile apps by reducing client-side complexity and ensuring that all long-running or stateful operations happen on the backend. This allows the Android and iOS apps to mirror the web experience without duplicating business logic.

## Key Advantages

### 1. Thin, Consistent Clients
Mobile apps no longer need to orchestrate multi-step workflows. Instead, they:

- Trigger an action (e.g., create business, enable module)
- Receive an immediate acknowledgment
- Poll or subscribe to status updates
- Render the final state once the backend completes the workflow

This keeps the mobile apps lightweight and aligned with the web version.

### 2. Unified Behavior Across Platforms
Because workflows are driven by backend events:

- Web, Android, and iOS all observe the same state transitions
- No platform drifts or inconsistent logic
- All clients reflect the same business rules automatically

The backend becomes the single source of truth for workflow behavior.

### 3. Better Performance on Mobile
Mobile devices avoid:

- Long-running API calls
- Chained requests
- Heavy computation
- State management for multi-step processes

Instead, the backend handles everything asynchronously, and the mobile apps simply react to updated state.

### 4. Real-Time or Near-Real-Time Updates
Mobile apps can subscribe to:

- WebSockets
- Server-Sent Events
- Push notifications
- Polling endpoints

This allows them to reflect backend events as they occur, mirroring the web dashboard experience.

### 5. Easier Offline Support
Because workflows are asynchronous:

- Mobile apps can queue actions locally
- Sync them when online
- Let the backend process them via events

The client does not need to manage workflow state.

### 6. Faster Feature Development
New backend event consumers can be added without modifying mobile apps. For example:

- Adding a new provisioning step
- Adding a new audit log entry
- Adding a new notification type

Mobile apps automatically benefit from backend improvements.

## Summary

The event-driven architecture enables Android and iOS apps to:

- Stay thin and consistent
- Avoid duplicating business logic
- React to backend-driven state changes
- Mirror the web experience with minimal effort
- Scale cleanly as the platform grows

This architecture is ideal for a multi-platform product family.

---

# 🌐 Diagram 1 - High-Level Architecture Diagram

```mermaid
flowchart LR
    subgraph Client["User Devices"]
        A[Browser]
    end

    subgraph Edge["AWS Edge Layer"]
        B[CloudFront CDN]
    end

    subgraph WebTier["EC2 Web Tier"]
        C[Nginx Reverse Proxy]
        D[BAU Admin App]
        E[BAU Main App]
    end

    subgraph Data["Data Layer"]
        F[(RDS Microsoft SQL)]
    end

    subgraph Storage["Static Assets"]
        G[S3 Bucket]
    end

    A --> B
    B --> C
    C --> D
    C --> E
    D --> F
    E --> F
    D --> G
    E --> G
```

This diagram illustrates the high-level architecture of the BAU platform, showing how user devices interact with the web tier, which in turn communicates with the data layer and static asset storage.

---

# 🚀 Diagram 2 - Deployment Pipeline (GitHub -> AWS -> EC2)

```mermaid
flowchart LR
    subgraph Dev["Developer Workflow"]
        A[Git Commit & Push]
        B[GitHub Actions CI/CD]
    end

    subgraph Artifact["Build & Artifact Storage"]
        C[Build .NET Apps]
        D[Upload ZIP to S3 Artifact Bucket]
    end

    subgraph Deploy["AWS CodeDeploy"]
        E[CodeDeploy Deployment Group]
        F[AppSpec.yml Lifecycle Hooks]
    end

    subgraph EC2["EC2 Instance"]
        G[Stop Existing Services]
        H[Extract New Release]
        I[Start Services]
        J[Nginx Serves Admin/Main]
    end

    A --> B
    B --> C
    C --> D
    D --> E
    E --> F
    F --> G
    G --> H
    H --> I
    I --> J
```

This diagram illustrates the deployment pipeline from code commit in GitHub to deployment on AWS EC2 instances using GitHub Actions and AWS CodeDeploy.

---

# 🛰️ Diagram 3 - Network Topology (VPC, Subnets, Routing)

```mermaid
flowchart TB
    subgraph VPC["AWS VPC"]
        
        subgraph PublicSubnet["Public Subnet"]
            IGW[Internet Gateway]
            NGINX[EC2 Instance<br/>Nginx + Admin/Main]
            SGWeb[SG: Web Tier]
        end

        subgraph PrivateSubnet["Private Subnet"]
            RDS[(RDS Microsoft SQL)]
            SGRDS[SG: Database]
        end

        subgraph Routing["Routing"]
            RT1[Route Table<br/>0.0.0.0/0 → IGW]
        end
    end

    IGW --> NGINX
    NGINX --> RDS

    NGINX -. inbound 443/80 .-> SGWeb
    RDS -. inbound 5432 .-> SGRDS

    RT1 --> PublicSubnet
```

This diagram illustrates the network topology of the BAU platform within an AWS VPC, showing public and private subnets, security groups, and routing configurations.

---

# 🔗 Diagram 4 - Service Dependency Graph

```mermaid
flowchart LR

    subgraph Frontend["Frontend Applications"]
        A[BAU Admin App]
        B[BAU Main App]
    end

    subgraph Backend["Backend Services"]
        C[BAU API Service]
        D[Auth Service]
        E[Metrics Collector]
        F[Logging Service - Serilog Sinks]
    end

    subgraph Data["Data Layer"]
        G[(RDS Microsoft SQL)]
        H[S3 Static Assets]
    end

    A --> C
    B --> C

    C --> D
    C --> F
    C --> E

    D --> G
    C --> G

    A --> H
    B --> H
```

This diagram illustrates the service dependencies within the BAU platform, showing how frontend applications interact with backend services and the data layer.

---

# 🗄️ Diagram 5 - Database Schema (ERD-Style Mermaid)

```mermaid
erDiagram

    USER {
        int Id PK
        string Email
        string PasswordHash
        string Role
        datetime CreatedAt
    }

    BUSINESS {
        int Id PK
        string Name
        string Type
        datetime CreatedAt
    }

    MODULE {
        int Id PK
        string Name
        string Category
    }

    SUBMODULE {
        int Id PK
        int ModuleId FK
        string Name
    }

    AUDITLOG {
        int Id PK
        int UserId FK
        string Action
        datetime Timestamp
    }

    USER ||--o{ BUSINESS : "owns"
    BUSINESS ||--o{ MODULE : "enables"
    MODULE ||--o{ SUBMODULE : "contains"
    USER ||--o{ AUDITLOG : "creates"
```

This diagram illustrates the core database schema of the BAU platform, showing entities such as User, Business, Module, Submodule, and AuditLog, along with their relationships.

---

# 🔐 Diagram 6 - User Login Sequence Flow

```mermaid
sequenceDiagram
    participant U as User
    participant A as Admin/Main App
    participant API as BAU API
    participant AUTH as Auth Service
    participant DB as RDS PostgreSQL

    U->>A: Submit login form
    A->>API: POST /auth/login (email, password)
    API->>AUTH: Validate credentials
    AUTH->>DB: Query user by email
    DB-->>AUTH: User record + password hash
    AUTH->>AUTH: Verify password
    AUTH-->>API: Auth result + JWT token
    API-->>A: Login success + token
    A-->>U: Redirect to dashboard
```

This diagram illustrates the sequence flow of a user login process in the BAU platform, showing interactions between the user, frontend app, API service, authentication service, and database.

---

# 🧩 Diagram 7 - Tech Stack Grid

```mermaid
flowchart LR
    subgraph Frontend["Frontend"]
        A[MVC / Blazor]
        B[TypeScript]
        C[Bootstrap + Custom Theme Engine]
    end

    subgraph Backend["Backend"]
        D[.NET 8 Web API]
        E[Auth Service]
        F[Serilog Logging]
    end

    subgraph Infrastructure["Infrastructure"]
        G[AWS EC2]
        H[AWS S3]
        I[AWS RDS Microsoft SQL]
        J[AWS CloudFront]
        K[AWS Route 53]
    end

    subgraph Deployment["CI/CD Pipeline"]
        L[GitHub Actions]
        M[CodeDeploy]
        N[S3 Artifact Bucket]
    end

    subgraph Tooling["Developer Tooling"]
        O[Docker]
        P[GitHub Projects]
        Q[VS Code / Rider]
    end

    A --> D
    A --> B
    B --> D
    D --> G
    E --> I
    F --> I
    G --> H
    L --> M
    M --> G
    N --> M
    O --> D
    P --> L
    Q --> D
```

This diagram illustrates the technology stack used in the BAU platform, categorizing components into frontend, backend, infrastructure, deployment, and developer tooling.

---

# 🌐 Diagram 8 - Nginx Reverse Proxy Routing

```mermaid
flowchart TB

    subgraph Client["User Browser"]
        A[HTTPS Request]
    end

    subgraph Nginx["Nginx Reverse Proxy on EC2"]
        B[/Location: /admin → Admin App/]
        C[/Location: / → Main App/]
        D[/Location: /api → BAU API/]
        E[/Static Files → S3/]
    end

    subgraph Apps["Application Layer"]
        F[BAU Admin App]
        G[BAU Main App]
        H[BAU API Service]
    end

    subgraph Data["Data Layer"]
        I[(RDS Microsoft SQL)]
        J[S3 Static Assets]
    end

    A --> B
    A --> C
    A --> D
    A --> E

    B --> F
    C --> G
    D --> H

    F --> I
    G --> I
    H --> I

    E --> J
```

This diagram illustrates how Nginx acts as a reverse proxy in the BAU platform, routing requests to the appropriate frontend applications and API service while serving static files from S3.

---

# 📊 Diagram 9 - System Metric Pipeline (Collectors -> API -> Dashboard)

```mermaid
flowchart LR

    subgraph System["EC2 System Metrics"]
        A[CPU Collector]
        B[Memory Collector]
        C[Disk Collector]
        D[Network Collector]
    end

    subgraph API["BAU API - Metrics Endpoint"]
        E[/POST /metrics/update/]
        F[/GET /metrics/live/]
    end

    subgraph Storage["In-Memory Cache / Metrics Store"]
        G[(Metrics Snapshot)]
    end

    subgraph Dashboard["BAU Admin Dashboard"]
        H[Live Charts<br/>CPU/Memory/Disk/Network]
        I[Health Cards]
    end

    A --> E
    B --> E
    C --> E
    D --> E

    E --> G
    F --> G

    G --> H
    G --> I
```

This diagram illustrates the system metric pipeline in the BAU platform, showing how system metrics are collected, sent to the API, stored, and displayed on the admin dashboard.

---

# 🧭 Diagram 10 - Provisioning Wizard Flow (Business Creation + Module Setup)

```mermaid
flowchart TB

    subgraph User["User"]
        U[Start Provisioning Wizard]
    end

    subgraph UI["Admin App - Provisioning Wizard"]
        A[Select Business Type]
        B[Choose Modules]
        C[Choose Submodules]
        D[Review & Confirm]
        E[POST /provision]
    end

    subgraph API["BAU API - Provisioning Service"]
        F[Validate Request]
        G[Create Business Record]
        H[Assign Modules]
        I[Assign Submodules]
        J[Generate Default Settings]
        K[Emit Audit Log]
    end

    subgraph DB["RDS Microsoft SQL"]
        L[(Business Table)]
        M[(Module Table)]
        N[(Submodule Table)]
        O[(Settings Table)]
        P[(AuditLog Table)]
    end

    U --> A
    A --> B
    B --> C
    C --> D
    D --> E

    E --> F
    F --> G
    G --> H
    H --> I
    I --> J
    J --> K

    G --> L
    H --> M
    I --> N
    J --> O
    K --> P
```

This diagram illustrates the provisioning wizard flow in the BAU platform, showing how a user interacts with the admin app to create a business and set up modules, and how the API processes the request and updates the database.

---

# 🗺️ Diagram 11 - API Routing Map (Domain-Organized Endpoints)

```mermaid
flowchart TB

    subgraph Auth["Auth Endpoints"]
        A1[/POST /auth/login/]
        A2[/POST /auth/refresh/]
        A3[/POST /auth/logout/]
    end

    subgraph Metrics["Metrics Endpoints"]
        M1[/POST /metrics/update/]
        M2[/GET /metrics/live/]
    end

    subgraph Provision["Provisioning Endpoints"]
        P1[/POST /provision/]
        P2[/GET /provision/modules/]
        P3[/GET /provision/submodules/]
    end

    subgraph Business["Business Endpoints"]
        B1[/GET /business/]
        B2[/POST /business/]
        B3[/PUT /business/:id/]
    end

    subgraph Modules["Module Endpoints"]
        MO1[/GET /modules/]
        MO2[/POST /modules/]
    end

    subgraph Submodules["Submodule Endpoints"]
        S1[/GET /submodules/]
        S2[/POST /submodules/]
    end

    subgraph Settings["Settings Endpoints"]
        ST1[/GET /settings/:businessId/]
        ST2[/PUT /settings/:businessId/]
    end

    subgraph Audit["Audit Log Endpoints"]
        AU1[/GET /audit/]
    end

    A1 --> M1
    A1 --> B1
    P1 --> B2
    P1 --> MO1
    P1 --> S1
    M1 --> M2
    B1 --> ST1
    ST1 --> AU1
```

This diagram illustrates the API routing map of the BAU platform, organizing endpoints by domain and showing their relationships.

---

# 🎨 Diagram 12 - Multi-Tenant Theme Engine Architecture

```mermaid
flowchart TB

    subgraph User["User"]
        U[Select Theme: Light, Dark, Brand]
    end

    subgraph Frontend["Admin/Main Frontend"]
        A[Load Theme Config]
        B[Apply CSS Variables]
        C[Render Components]
    end

    subgraph Engine["Universal Theme Engine"]
        D[Palette Definitions - Primary Secondary Accent]
        E[Semantic Tokens - Success Warning Error]
        F[Interaction Layer - Hover Focus Active Pressed]
        G[Runtime Switcher]
    end

    subgraph Output["Rendered UI"]
        H[Buttons Inputs Cards]
        I[Sidebar Navigation Panels]
        J[Charts Metrics Tables]
    end

    U --> A
    A --> G
    G --> D
    G --> E
    G --> F

    D --> B
    E --> B
    F --> B

    B --> C
    C --> H
    C --> I
    C --> J
```

This diagram illustrates the architecture of the multi-tenant theme engine in the BAU platform, showing how user-selected themes are applied to frontend components through a universal theme engine.

---

# 🧱 Diagram 13 - Admin Dashboard Layout Architecture

```mermaid
flowchart TB

    subgraph Layout["Admin Dashboard Layout"]
        A[Sidebar Navigation]
        B[Top Bar]
        C[Content Router]
        D[Main Content Panel]
    end

    subgraph Widgets["Dashboard Widgets"]
        E[Health Cards]
        F[Live Metrics Charts]
        G[Recent Activity]
        H[System Status]
    end

    subgraph Services["Backend Services"]
        I[/GET /metrics/live/]
        J[/GET /audit/]
        K[/GET /settings/:businessId/]
    end

    A --> C
    B --> C
    C --> D

    D --> E
    D --> F
    D --> G
    D --> H

    F --> I
    G --> J
    E --> K
```

This diagram illustrates the architecture of the admin dashboard layout in the BAU platform, showing how the layout components interact with backend services to display various widgets.

---

# 🪵 Diagram 14 - Logging & Diagnostics Pipeline

```mermaid
flowchart LR

    subgraph Apps["Applications"]
        A[BAU Admin App]
        B[BAU Main App]
        C[BAU API Service]
    end

    subgraph Logger["Serilog Pipeline"]
        D[Structured Log Event]
        E[Enrichers - RequestId UserId Machine]
        F[Formatters - JSON Text]
    end

    subgraph Sinks["Serilog Sinks"]
        G[File Sink - /logs/app.log]
        H[Console Sink]
        I[Rolling File Sink]
    end

    subgraph Storage["Log Storage"]
        J[(EC2 File System)]
        K[(Future: S3 Log Archive)]
        L[(Future: CloudWatch Logs)]
    end

    A --> D
    B --> D
    C --> D

    D --> E
    E --> F

    F --> G
    F --> H
    F --> I

    G --> J
    I --> J

    J --> K
    J --> L
```

This diagram illustrates the logging and diagnostics pipeline in the BAU platform, showing how applications generate structured log events that are processed through Serilog and stored for analysis.

---

# ⚙️ Diagram 15 - Configuration & Settings Flow

```mermaid
flowchart TB

    subgraph User["User"]
        U[Open Settings Panel]
        U2[Update Setting Value]
    end

    subgraph UI["Admin App - Settings UI"]
        A[Load Settings]
        B[Display Editable Fields]
        C[POST /settings/update]
        D[Refresh UI State]
    end

    subgraph API["BAU API - Settings Service"]
        E[Validate Input]
        F[Update Settings Record]
        G[Return Updated Snapshot]
    end

    subgraph DB["RDS PostgreSQL"]
        H[(Settings Table)]
        I[(Business Table)]
    end

    U --> A
    A --> B
    B --> U2
    U2 --> C

    C --> E
    E --> F
    F --> H
    F --> I
    F --> G

    G --> D
```

This diagram illustrates the configuration and settings flow in the BAU platform, showing how users interact with the settings UI, how the API processes updates, and how changes are stored in the database.

---

# 🔑 Diagram 16 - Authentication Token Lifecycle

```mermaid
flowchart LR

    subgraph Client["Admin/Main App"]
        A[User Logs In]
        B[Store JWT in Memory]
        C[Attach JWT to Requests]
        D[Token Expired]
        E[Request Refresh Token]
    end

    subgraph API["BAU API Gateway"]
        F[Validate JWT]
        G[Reject or Forward]
        H[Forward to Auth Service]
    end

    subgraph Auth["Auth Service"]
        I[Verify Credentials]
        J[Generate JWT]
        K[Generate Refresh Token]
        L[Validate Refresh Token]
        M[Revoke Tokens]
    end

    subgraph DB["RDS PostgreSQL"]
        N[(Users)]
        O[(RefreshTokens)]
        P[(RevokedTokens)]
    end

    A --> I
    I --> N
    I --> J
    I --> K

    J --> B
    B --> C
    C --> F

    F --> G
    G --> H

    D --> E
    E --> L
    L --> O
    L --> J

    M --> P
```

This diagram illustrates the authentication token lifecycle in the BAU platform, showing how users log in, how JWTs are managed, and how tokens are refreshed and revoked through the API and Auth Service.

---

# 📦 Diagram 17 - Static Assets Delivery Pipeline

```mermaid
flowchart LR

    subgraph Build["Build System"]
        A[Frontend Build Output]
        B[Bundle JS CSS Images]
        C[Upload to S3 Bucket]
    end

    subgraph S3["AWS S3 - Static Assets"]
        D[Versioned Asset Files]
    end

    subgraph CDN["AWS CloudFront CDN"]
        E[Global Edge Cache]
    end

    subgraph Server["EC2 + Nginx"]
        F[Nginx Reverse Proxy]
        G[Fallback to S3 for Assets]
    end

    subgraph Client["User Browser"]
        H[Request Admin/Main App]
        I[Load JS CSS Images]
    end

    A --> B
    B --> C
    C --> D
    D --> E

    H --> F
    F --> I
    I --> E
    E --> D
```

This diagram illustrates the static assets delivery pipeline in the BAU platform, showing how frontend assets are built, stored in S3, cached in CloudFront, and served to users via Nginx.

---

# 🚀 Diagram 18 - Full Deployment Pipeline (GitHub -> AWS -> EC2 -> Nginx)

```mermaid
flowchart LR

    subgraph Dev["Developer"]
        A[Commit Code]
        B[Push to GitHub]
    end

    subgraph CI["GitHub Actions"]
        C[Run Tests]
        D[Build Artifacts]
        E[Upload ZIP to S3 Artifact Bucket]
        F[Trigger CodeDeploy Deployment]
    end

    subgraph CD["AWS CodeDeploy"]
        G[Download Artifact ZIP]
        H[Run AppSpec Hooks]
        I[Deploy to EC2 Target]
    end

    subgraph EC2["EC2 Instance"]
        J[Extract Build]
        K[Restart Services]
        L[Reload Nginx]
    end

    subgraph Runtime["Running System"]
        M[Admin App]
        N[Main App]
        O[BAU API]
    end

    A --> B
    B --> C
    C --> D
    D --> E
    E --> F

    F --> G
    G --> H
    H --> I

    I --> J
    J --> K
    K --> L

    L --> M
    L --> N
    L --> O
```

This diagram illustrates the full deployment pipeline of the BAU platform, showing the flow from code commit in GitHub through CI/CD processes to deployment on AWS EC2 instances and service restarts.

---

# 🛠️ Diagram 19 - Error Handling & Recovery Flow

```mermaid
flowchart TB

    subgraph UI["Admin/Main Frontend"]
        A[User Action]
        B[API Request]
        C[Handle Error Response]
        D[Show Toast or Modal]
    end

    subgraph API["BAU API"]
        E[Validate Input]
        F[Execute Handler]
        G[Catch Exception]
        H[Return Error DTO]
    end

    subgraph Services["Backend Services"]
        I[Business Logic]
        J[Database Query]
        K[External Call]
    end

    subgraph Logging["Serilog Pipeline"]
        L[Log Error Event]
        M[Write to File Sink]
    end

    subgraph Recovery["Recovery Options"]
        N[Retry Operation]
        O[Fallback Value]
        P[Graceful Degradation]
    end

    A --> B
    B --> E
    E --> F
    F --> I
    I --> J
    I --> K

    F --> G
    G --> L
    L --> M
    G --> H

    H --> C
    C --> D

    C --> N
    C --> O
    C --> P
```

This diagram illustrates the error handling and recovery flow in the BAU platform, showing how errors are propagated from user actions through the API and backend services, logged, and handled gracefully in the frontend.

---

# 🧩 Diagram 20 - Module & Submodule Heirarchy (ERP Structure)

```mermaid
flowchart TB

    subgraph BusinessTypes["Business Types"]
        A[Retail]
        B[Manufacturing]
        C[Logistics]
        D[Healthcare]
        E[Hospitality]
        F[Professional Services]
    end

    subgraph Modules["Core Modules"]
        M1[Inventory]
        M2[Sales]
        M3[Purchasing]
        M4[HR]
        M5[Finance]
        M6[Operations]
    end

    subgraph Submodules["Submodules"]
        S1[Inventory - Items]
        S2[Inventory - Stock Levels]
        S3[Sales - Orders]
        S4[Sales - Customers]
        S5[Purchasing - Vendors]
        S6[Purchasing - PO Management]
        S7[HR - Employees]
        S8[HR - Time Tracking]
        S9[Finance - Invoices]
        S10[Finance - Payments]
        S11[Operations - Workflows]
        S12[Operations - Schedules]
    end

    A --> M1
    A --> M2
    A --> M3

    B --> M1
    B --> M3
    B --> M6

    C --> M2
    C --> M6

    D --> M4
    D --> M5

    E --> M2
    E --> M4

    F --> M2
    F --> M5

    M1 --> S1
    M1 --> S2

    M2 --> S3
    M2 --> S4

    M3 --> S5
    M3 --> S6

    M4 --> S7
    M4 --> S8

    M5 --> S9
    M5 --> S10

    M6 --> S11
    M6 --> S12
```

This diagram illustrates the module and submodule hierarchy in the BAU platform, showing how different business types relate to core modules and their respective submodules.

---

# 🌐 Diagram 21 - Full System Overview (All Components)

```mermaid
flowchart TB

    subgraph Client["User Devices"]
        C1[Browser - Admin]
        C2[Browser - Main]
    end

    subgraph Frontend["Frontend Layer"]
        F1[Admin App - MVC]
        F2[Main App - Blazor]
        F3[Theme Engine]
    end

    subgraph Nginx["EC2 - Nginx Reverse Proxy"]
        N1[/Route: /admin → Admin App/]
        N2[/Route: / → Main App/]
        N3[/Route: /api → BAU API/]
        N4[/Static Assets → S3/]
    end

    subgraph API["Backend API Layer"]
        A1[BAU API Service]
        A2[Auth Service]
        A3[Metrics Endpoint]
        A4[Provisioning Service]
    end

    subgraph Services["Internal Services"]
        S1[Metrics Collectors]
        S2[Serilog Logging]
        S3[Settings Manager]
    end

    subgraph Data["Data Layer"]
        D1[(RDS Microsoft SQL)]
        D2[(S3 Static Assets)]
        D3[(Logs on EC2)]
    end

    subgraph Deployment["CI/CD Pipeline"]
        P1[GitHub Actions]
        P2[S3 Artifact Bucket]
        P3[CodeDeploy]
    end

    C1 --> F1
    C2 --> F2

    F1 --> N1
    F2 --> N2
    F1 --> N4
    F2 --> N4

    N3 --> A1

    A1 --> A2
    A1 --> A3
    A1 --> A4

    A3 --> S1
    A1 --> S2
    A1 --> S3

    A1 --> D1
    A2 --> D1
    A4 --> D1

    N4 --> D2
    S2 --> D3

    P1 --> P2
    P2 --> P3
    P3 --> Nginx
```

This diagram illustrates the full system overview of the BAU platform, showing all major components from user devices through frontend and backend layers to data storage and deployment pipelines.

---

# 📱 Diagram 22 - Future Mobile Architecture (Android & iOS)

```mermaid
flowchart LR

    subgraph Mobile["BAU Mobile Apps"]
        M1[Android App]
        M2[iOS App]
        M3[Shared Theme Engine]
        M4[Local Storage - Secure]
    end

    subgraph Gateway["API Gateway Layer"]
        G1[/Route: /api → BAU API/]
        G2[/Auth Token Validation/]
    end

    subgraph API["Backend Services"]
        A1[BAU API Service]
        A2[Auth Service]
        A3[Settings Service]
        A4[Metrics Service]
    end

    subgraph Data["Data Layer"]
        D1[(RDS Microsoft SQL)]
        D2[(S3 Assets)]
    end

    subgraph Push["Notifications"]
        P1[AWS SNS - Mobile Push]
    end

    M1 --> G1
    M2 --> G1

    M1 --> M3
    M2 --> M3

    M1 --> M4
    M2 --> M4

    G1 --> A1
    G2 --> A2

    A1 --> D1
    A2 --> D1
    A3 --> D1

    A1 --> D2

    A1 --> P1
    P1 --> M1
    P1 --> M2
```

This diagram illustrates the future mobile architecture for the BAU platform, showing how Android and iOS apps interact with the API gateway, backend services, data layer, and push notification system.

---

### TODO

```
mobile flows,
offline sync,
database schema,
request lifecycle
```

---
