# Business As Usual - AI Assistant Context Guide

**Document Version:** 2.0  
**Last Updated:** January 2025  
**Purpose:** Guide GitHub Copilot in understanding the codebase architecture and patterns  
**Solution Version:** ASP.NET Core 9.0

---

## 🎯 Architectural Vision & Philosophy

**Business As Usual** is being built as a **microservices architecture** with a unique twist: each service is self-contained and provides:

1. **API Endpoints** - RESTful services for data operations
2. **Web UI Components** - Blazor components that are **injected** into the main web application
3. **Mobile UI Specifications** - Contracts/models that **direct** Android and iOS apps on UI patterns to follow

### Core Principle: "Service = API + UI + Mobile Contract"

Each business module (HR, Accounting, Inventory, etc.) should be:
- **Independently deployable** as a microservice
- **UI-aware** - ships its own Blazor components
- **Mobile-friendly** - provides UI models/specifications for mobile apps
- **Self-contained** - owns its data, logic, and presentation

### Key Technologies
- **Framework:** .NET 9.0
- **Architecture:** Microservices + Micro-Frontends + Clean Architecture
- **Backend:** ASP.NET Core Web API (per service)
- **Web Frontend:** Blazor Server with **dynamic component injection**
- **Admin Portal:** ASP.NET Core Razor Pages
- **Mobile:** Native Android/iOS apps following service-provided UI contracts
- **Database:** Microsoft SQL Server (multi-tenant, database-per-tenant)
- **Cloud:** AWS (EC2, RDS, S3, CloudWatch, Secrets Manager)
- **Containerization:** Docker + Docker Compose (per service)
- **CI/CD:** GitHub Actions
- **Logging:** Serilog (structured logging)
- **UI Framework:** MudBlazor (for Blazor components)

---

## 🏗️ Microservices Architecture Blueprint

### Vision: Self-Contained Services with UI

Each business module (e.g., HR, Accounting, Inventory) will be structured as:

```
HR.Service/
│
├── HR.API/                    # REST API endpoints
│   ├── Controllers/           # HRController, EmployeesController, etc.
│   ├── Program.cs             # Service host
│   └── Dockerfile             # Independent deployment
│
├── HR.Application/            # Business logic & use cases
│   ├── Commands/              # CQRS commands
│   ├── Queries/               # CQRS queries
│   └── Services/              # Business services
│
├── HR.Domain/                 # Domain entities & interfaces
│   ├── Entities/              # Employee, Department, etc.
│   └── ValueObjects/          # Domain values
│
├── HR.Infrastructure/         # Data access & external services
│   ├── Persistence/           # EF Core context
│   └── Repositories/          # Data repositories
│
├── HR.Web/                    # Blazor UI Components (injected into main app)
│   ├── Components/            # Razor components
│   │   ├── EmployeeList.razor
│   │   ├── EmployeeDetail.razor
│   │   └── DepartmentCard.razor
│   ├── Pages/                 # Full page components
│   │   ├── Employees.razor
│   │   └── Dashboard.razor
│   └── Services/              # UI-specific services
│       └── HRUIService.cs
│
└── HR.Contracts/              # Mobile UI contracts & models
    ├── UIModels/              # View models for mobile
    │   ├── EmployeeViewModel.cs
    │   └── DepartmentViewModel.cs
    ├── Specifications/        # UI behavior specifications
    │   ├── EmployeeListSpec.cs
    │   └── EmployeeFormSpec.cs
    └── Navigation/            # Navigation contracts
        └── HRNavigationMap.cs
```

### Component Injection Strategy

**Web Application** (`BusinessAsUsual.Web`) acts as a **shell** that:
1. Discovers available services at runtime
2. **Dynamically loads** Blazor components from service assemblies
3. Registers service-specific routes
4. Injects components into navigation menus

**Example:**
```csharp
// In service: HR.Web/Components/EmployeeList.razor
@inherits ModuleComponentBase
// This component gets injected into BusinessAsUsual.Web at runtime

// In main app: BusinessAsUsual.Web/Program.cs
builder.Services.AddModuleDiscovery(); // Discovers HR.Web.dll
builder.Services.RegisterModuleComponents(); // Loads HR components
```

### Mobile UI Contract Pattern

Each service provides **UI specifications** that mobile apps consume:

**Service defines:**
```csharp
// HR.Contracts/UIModels/EmployeeViewModel.cs
public class EmployeeViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string PhotoUrl { get; set; }
    public string Department { get; set; }
}

// HR.Contracts/Specifications/EmployeeListSpec.cs
public class EmployeeListSpec
{
    public string Title => "Employees";
    public string SearchPlaceholder => "Search employees...";
    public List<ColumnDefinition> Columns { get; set; }
    public List<ActionButton> Actions { get; set; }
}
```

**Mobile app consumes:**
```kotlin
// Android app fetches UI spec from API
val spec = api.getEmployeeListSpec()
// Renders native UI based on specification
renderListView(spec, employeeData)
```

---

## 🎭 Current State vs. Target State

### Current State (Monolithic)
```
BusinessAsUsual.Web ──────► BusinessAsUsual.API ──────► Database
     (Blazor)                  (Single API)           (SQL Server)
```

### Target State (Microservices + Micro-Frontends)
```
                                  ┌──────────────┐
                                  │  API Gateway │
                                  └───────┬──────┘
                                          │
              ┌───────────────────────────┼───────────────────────────┐
              │                           │                           │
         ┌────▼─────┐               ┌────▼─────┐               ┌────▼─────┐
         │ HR.API   │               │ Acct.API │               │ Inv.API  │
         └────┬─────┘               └────┬─────┘               └────┬─────┘
              │                           │                           │
         ┌────▼─────┐               ┌────▼─────┐               ┌────▼─────┐
         │  HR DB   │               │ Acct DB  │               │  Inv DB  │
         └──────────┘               └──────────┘               └──────────┘

                    ┌────────────────────────────────┐
                    │  BusinessAsUsual.Web (Shell)   │
                    │  Injects: HR.Web + Acct.Web +  │
                    │           Inv.Web components    │
                    └────────────────────────────────┘

                    ┌────────────────────────────────┐
                    │  Android/iOS Apps              │
                    │  Follows: HR.Contracts +       │
                    │           Acct.Contracts UI    │
                    └────────────────────────────────┘
```

---

## 📋 Migration Strategy: Monolith → Microservices

### Phase 1: Extract First Service (HR Module) ✅ Next Step
1. Create `services/HR/` directory structure
2. Move HR domain models from `BusinessAsUsual.Domain/Modules/HR.Domain/`
3. Create `HR.API` with controllers
4. Create `HR.Web` with Blazor components
5. Create `HR.Contracts` with mobile UI models
6. Implement component injection in `BusinessAsUsual.Web`
7. Deploy HR service independently

### Phase 2: Extract Additional Services
- Accounting Service
- Inventory Service
- Timekeeping Service
- Orders Service

### Phase 3: Decompose Core API
- Migrate shared platform services
- Implement API Gateway
- Service discovery & registration

---

## 🎯 Key Patterns & Conventions (For AI Assistant)

### When User Says "Add a new module called X":

1. **Create service directory structure:**
   ```
   services/X/
   ├── X.API/
   ├── X.Application/
   ├── X.Domain/
   ├── X.Infrastructure/
   ├── X.Web/           # Blazor components
   └── X.Contracts/     # Mobile UI specs
   ```

2. **API Project (`X.API/Program.cs`):**
   - Use minimal API or controllers
   - Register service-specific dependencies
   - Configure Swagger for this service
   - Add health checks
   - Add metrics middleware

3. **Web Components (`X.Web/`):**
   - Create Razor components that inherit from `ModuleComponentBase`
   - Follow MudBlazor component patterns
   - Register components for injection into main app
   - Include `_Imports.razor` with necessary namespaces

4. **Mobile Contracts (`X.Contracts/`):**
   - Create `UIModels/` - ViewModels for mobile consumption
   - Create `Specifications/` - UI behavior specifications
   - Create `Navigation/` - Navigation contracts
   - All models should be simple POCOs (JSON serializable)

### When User Says "Add a feature to module X":

1. **Check if service exists:** Look in `services/X/` or `backend/BusinessAsUsual.Domain/Modules/X.Domain/`
2. **Add domain entity:** `X.Domain/Entities/`
3. **Add application logic:** `X.Application/Services/` or `X.Application/Commands/`
4. **Add API endpoint:** `X.API/Controllers/`
5. **Add Blazor component:** `X.Web/Components/` or `X.Web/Pages/`
6. **Update mobile contract:** `X.Contracts/UIModels/`

### When User Says "Fix the UI for X":

- **If it's Blazor:** Look in `frontend/BusinessAsUsual.Web/` (current) or `services/X/X.Web/` (future)
- **If it's Admin Portal:** Look in `frontend/BusinessAsUsual.Admin/Areas/Admin/`
- **If it's mobile:** Update contracts in `services/X/X.Contracts/`

### When User Says "The API isn't working for X":

- **Current state:** Check `backend/BusinessAsUsual.API/Controllers/`
- **Future state:** Check `services/X/X.API/Controllers/`
- Look for related application logic in `backend/BusinessAsUsual.Application/` or `services/X/X.Application/`

---

## 🛠️ Current Project Structure (Transitioning)

### Backend Projects

#### **BusinessAsUsual.API** - Monolithic API (Being Decomposed)
**Current Role:** Single API serving all modules  
**Future Role:** API Gateway + Platform services only  
**Port:** 5000 (Docker), 5001 (HTTPS local)

**Key Controllers:**
- `AuthController` - Authentication (will remain in platform)
- `ProvisioningApiController` - Tenant provisioning (platform)
- `MetricsController` - Metrics (platform)
- `HealthController` - Health checks (platform)
- `CompanyProfileController` - Company management (platform)
- Module-specific controllers (will move to services)

**Startup Configuration:**
- CORS for web and admin frontends
- AWS CloudWatch metrics (production)
- SQL Server with retry logic
- Swagger UI

#### **BusinessAsUsual.Application** - Application Layer
**Purpose:** Business logic & use cases  
**Structure:** Module-specific application logic

**Current Folders:**
- `Common/` - Shared interfaces
- `Services/Provisioning/` - Multi-tenant provisioning
- `Modules/HR.Application/` - HR business logic (**ready to extract**)
- `Modules/Accounting.Application/` - Accounting logic (**ready to extract**)
- `Modules/Inventory.Application/` - Inventory logic (**ready to extract**)

**Migration Pattern:**
- Module folders will become `services/X/X.Application/`

#### **BusinessAsUsual.Domain** - Domain Layer
**Purpose:** Domain entities and interfaces (Clean Architecture)

**Current Folders:**
- `Core/` - Core domain models
- `Modules/HR.Domain/` - HR entities (**ready to extract**)
- `Modules/Accounting.Domain/` - Accounting entities (**ready to extract**)
- `Modules/Inventory.Domain/` - Inventory entities (**ready to extract**)
- `Modules/Timekeeping.Domain/` - Timekeeping entities (**ready to extract**)
- `Modules/Orders.Domain/` - Order entities (**ready to extract**)

**Migration Pattern:**
- Module folders will become `services/X/X.Domain/`

#### **BusinessAsUsual.Infrastructure** - Infrastructure Layer
**Purpose:** Data access, AWS services, external integrations

**Dependencies:**
- EF Core 9.0
- AWS SDK (CloudWatch, Secrets Manager)
- SQL Server Management Objects

**Current Folders:**
- `Database/` - EF Core contexts
- `Monitoring/` - CloudWatch metrics
- `Provisioning/` - Tenant provisioning
- `Modules/HR.Infrastructure/` (**ready to extract**)
- `Modules/Accounting.Infrastructure/` (**ready to extract**)
- `Modules/Inventory.Infrastructure/` (**ready to extract**)

**Migration Pattern:**
- Module folders will become `services/X/X.Infrastructure/`

---

### Frontend Projects

#### **BusinessAsUsual.Web** - Blazor Shell Application
**Technology:** Blazor Server (ASP.NET Core 9.0)  
**Port:** 3000 (Docker), 7229 (HTTPS local)  
**Current Role:** Full-featured web application  
**Future Role:** **Shell that dynamically injects service UI components**

**Key Architecture Changes Needed:**
- Implement **dynamic component loading** from service assemblies
- **Module discovery** at startup (scan for `X.Web.dll` assemblies)
- **Component registration** from discovered modules
- **Dynamic routing** based on loaded components
- **Shared layout** with module-specific content injection

**Current Structure:**
- `Components/Layout/` - MainLayout, navigation, topbar
- `Modules/` - Built-in modules (will become injected)
  - `HR/Services/` - HR services (**move to HR.Web**)
  - `_Shared/` - Module base classes (**keep for shell**)
- `Themes/` - Theme system (ThemeContext, ThemeRegistry)
- `Services/` - Shell services (PageHeaderService, LoggingCircuitHandler)

**Key Services:**
- `ThemeContext` - Singleton for global theme
- `PageHeaderService` - Page header management
- `LoggingCircuitHandler` - Blazor circuit logging

**Component Injection Pattern (To Implement):**
```csharp
// Program.cs
builder.Services.AddModuleDiscovery(options => 
{
    options.ModuleAssemblyPath = "modules/";
    options.SearchPattern = "*.Web.dll";
});

// At runtime:
// 1. Discover HR.Web.dll, Accounting.Web.dll, etc.
// 2. Register Razor components from each assembly
// 3. Add routes from module metadata
// 4. Inject components into navigation
```

**Navigation Injection:**
```razor
<!-- MainLayout.razor -->
<MudNavMenu>
    @foreach (var module in LoadedModules)
    {
        <MudNavGroup Title="@module.Name" Icon="@module.Icon">
            @foreach (var page in module.Pages)
            {
                <MudNavLink Href="@page.Route">@page.Title</MudNavLink>
            }
        </MudNavGroup>
    }
</MudNavMenu>
```

#### **BusinessAsUsual.Admin** - Admin Portal
**Technology:** ASP.NET Core Razor Pages + MVC  
**Port:** 8080 (Docker), 7238 (HTTPS local)  
**Purpose:** Platform administration (stays monolithic)

**Key Features:**
- System monitoring (CloudWatch integration)
- Tenant provisioning
- Log viewing (Serilog + CloudWatch)
- Metrics dashboard
- Company settings
- Audit logs
- Service health monitoring

**Structure:**
- `Areas/Admin/Controllers/` - Admin controllers
- `Areas/Admin/Models/` - ViewModels
- `Areas/Admin/ViewComponents/` - Reusable components
- `Services/` - Admin services (MonitoringService, LogQueryService)
- `Hubs/` - SignalR hubs (SmartCommitHub)
- `Logging/` - Serilog configuration

**Key Controllers:**
- `DashboardController` - Main dashboard
- `MonitoringController` - Service monitoring (**will monitor microservices**)
- `LogsController` - Log viewer
- `CompanyController` - Tenant management
- `SettingsController` - System settings

**Future Enhancement:**
- Dashboard should show health of all microservices
- Monitoring should support distributed tracing
- Logs should aggregate from all services

---

### Shared Projects

#### **BusinessAsUsual.Core** - Module Catalog & Shared Definitions
**Technology:** .NET 9.0 Class Library  
**Purpose:** Central module registry and shared contracts

**Key Components:**
- `ModuleDefinition` - Metadata for business modules
- `SubmoduleDefinition` - Submodule metadata
- `ModuleCatalog` - Registry of all 50+ modules across 10 categories

**Module Categories:**
1. **Platform** - User Management, Audit Logs, Notifications, Reporting, Integrations, Settings
2. **Financial** - Accounting, GL, AR, AP, Billing, Invoicing, Payments
3. **Sales & CRM** - Leads, Opportunities, Quotes, Customers, Sales Orders
4. **Operations** - Inventory, Procurement, Warehouse, Manufacturing
5. **HR & Payroll** - Employees, Attendance, Payroll, Performance
6. **Service Management** - Work Orders, Service Requests, Field Service
7. **Projects** - Project Management, Time Tracking, Resources
8. **Assets & Facilities** - Asset Management, Maintenance
9. **Marketing** - Campaigns, Email Marketing, Events
10. **Support** - Tickets, Knowledge Base, SLA

**Future Enhancement:**
- Add `IModuleMetadata` interface for service discovery
- Include route definitions for dynamic loading
- Component registration helpers

#### **BusinessAsUsual.Tests** - Test Suite
**Technology:** .NET 9.0 Test Project  
**Purpose:** Unit, integration, and end-to-end tests

**Future Structure:**
```
BusinessAsUsual.Tests/
├── Unit/
│   ├── HR.Domain.Tests/
│   ├── Accounting.Domain.Tests/
│   └── Platform.Tests/
├── Integration/
│   ├── HR.API.Tests/
│   ├── Accounting.API.Tests/
│   └── Platform.API.Tests/
└── E2E/
    ├── Web.Tests/ (Playwright/Selenium)
    └── API.Tests/   (REST Assured)
```

---

## 🔄 Service Communication Patterns

### API-to-API Communication (Microservices)

**Pattern:** REST + Event-Driven

```
HR.API ──REST──► Accounting.API  (Sync: Get employee cost center)
HR.API ──Event─► Accounting.API  (Async: Employee hired event)
```

**When to use REST:**
- Real-time queries (e.g., "Get employee salary")
- Synchronous operations (e.g., "Validate customer exists")

**When to use Events:**
- State changes (e.g., "Employee terminated")
- Cross-module notifications (e.g., "Invoice paid")
- Eventual consistency scenarios

**Future Implementation:**
- **Service Bus:** Azure Service Bus or AWS SQS/SNS
- **Event Store:** For event sourcing
- **API Gateway:** For service routing

### Web-to-API Communication

**Current:** Direct HTTP calls to `BusinessAsUsual.API`  
**Future:** Shell loads service components, components call their own APIs

```
Blazor Component (HR.Web) ──HTTP──► HR.API
Blazor Component (Acct.Web) ──HTTP──► Accounting.API
```

### Mobile-to-API Communication

**Pattern:** REST + UI Contract Discovery

```
1. Android App ──GET /api/hr/ui-spec──► HR.API
2. HR.API responds with UI specification (JSON)
3. Android renders native UI based on spec
4. Android ──POST /api/hr/employees──► HR.API
```

**Example Flow:**
```json
// GET /api/hr/employees/ui-spec
{
  "listView": {
    "title": "Employees",
    "searchable": true,
    "columns": [
      { "field": "fullName", "label": "Name", "sortable": true },
      { "field": "department", "label": "Department", "sortable": true }
    ],
    "actions": [
      { "label": "Add Employee", "route": "/employees/new" },
      { "label": "Export", "action": "export" }
    ]
  }
}
```

---

## 🎨 UI Component Injection Deep Dive

### How Web Shell Loads Service Components

#### Step 1: Service Discovery at Startup
```csharp
// BusinessAsUsual.Web/Program.cs
builder.Services.AddModuleAssemblyScanning(options =>
{
    options.ScanPath = Path.Combine(AppContext.BaseDirectory, "modules");
    options.AssemblyPattern = "*.Web.dll";
});
```

#### Step 2: Component Registration
```csharp
// Discovered assemblies: HR.Web.dll, Accounting.Web.dll, etc.
foreach (var assembly in discoveredAssemblies)
{
    // Register all Razor components
    var components = assembly.GetTypes()
        .Where(t => t.IsSubclassOf(typeof(ComponentBase)));

    foreach (var component in components)
    {
        builder.Services.AddScoped(component);
    }

    // Register routes from module metadata
    var moduleMetadata = assembly.GetCustomAttribute<ModuleMetadataAttribute>();
    if (moduleMetadata != null)
    {
        RouteRegistry.RegisterRoutes(moduleMetadata.Routes);
    }
}
```

#### Step 3: Dynamic Rendering
```razor
<!-- BusinessAsUsual.Web/Components/Layout/MainLayout.razor -->
@inject IModuleRegistry ModuleRegistry

<MudLayout>
    <MudAppBar>
        <!-- Navigation dynamically built from loaded modules -->
        @foreach (var module in ModuleRegistry.GetLoadedModules())
        {
            <MudMenu Label="@module.Name">
                @foreach (var route in module.Routes)
                {
                    <MudMenuItem Href="@route.Path">@route.Label</MudMenuItem>
                }
            </MudMenu>
        }
    </MudAppBar>

    <MudMainContent>
        @Body  <!-- Dynamically loaded component renders here -->
    </MudMainContent>
</MudLayout>
```

### Service Component Structure

**Each service's `X.Web` project:**
```
HR.Web/
├── Components/
│   ├── EmployeeCard.razor       # Reusable component
│   ├── DepartmentSelector.razor # Reusable component
│   └── EmployeeForm.razor       # Complex form
├── Pages/
│   ├── Employees.razor          # Full page (route: /hr/employees)
│   ├── EmployeeDetail.razor     # Detail page (route: /hr/employees/{id})
│   └── Dashboard.razor          # Module dashboard
├── Services/
│   └── HRUIService.cs           # UI-specific logic
├── _Imports.razor               # Shared imports
├── ModuleMetadata.cs            # Routes & metadata
└── HR.Web.csproj
```

**ModuleMetadata.cs Example:**
```csharp
[assembly: ModuleMetadata(
    Name = "Human Resources",
    Icon = "mdi-account-group",
    Routes = new[]
    {
        new RouteDefinition("/hr", "Dashboard", typeof(Pages.Dashboard)),
        new RouteDefinition("/hr/employees", "Employees", typeof(Pages.Employees)),
        new RouteDefinition("/hr/employees/{id}", "Employee Detail", typeof(Pages.EmployeeDetail))
    }
)]
```

---

## 🔄 Complete Data Flow Architecture

### Current State: Monolithic Flow
```
┌──────────────┐          ┌──────────────┐
│ Blazor Web   │          │ Admin Portal │
│ (Port 3000)  │◄────────►│ (Port 8080)  │
└──────┬───────┘   CORS   └──────┬───────┘
       │                         │
       │      HTTP/REST          │
       └───────────┬─────────────┘
                   │
                   ▼
         ┌─────────────────┐
         │ Monolithic API  │
         │   (Port 5000)   │
         └────────┬────────┘
                  │
                  ▼
         ┌─────────────────┐
         │   SQL Server    │
         │  (Multi-tenant) │
         └─────────────────┘
```

### Target State: Microservices Flow
```
┌────────────────────────────────────────────────────────┐
│         Blazor Web Shell (Port 3000)                   │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐             │
│  │ HR.Web   │  │ Acct.Web │  │ Inv.Web  │  (Injected) │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘             │
└───────┼────────────┼─────────────┼───────────────────┘
        │            │             │
        │    HTTP/REST (Direct)    │
        │            │             │
   ┌────▼────┐  ┌───▼─────┐  ┌───▼─────┐
   │ HR.API  │  │Acct.API │  │ Inv.API │
   │ :5001   │  │ :5002   │  │ :5003   │
   └────┬────┘  └────┬────┘  └────┬────┘
        │            │            │
   ┌────▼────┐  ┌───▼─────┐  ┌───▼─────┐
   │  HR DB  │  │ Acct DB │  │ Inv DB  │
   └─────────┘  └─────────┘  └─────────┘

┌────────────────────────────────────────────────────────┐
│              Android/iOS Apps                          │
│  1. GET /api/hr/ui-spec → UI Contract                 │
│  2. Render native UI                                   │
│  3. POST /api/hr/employees → Business operation        │
└────────────────────────────────────────────────────────┘
```

### Service-to-Service Communication
```
┌─────────────────────────────────────────────────┐
│  HR Service needs Accounting data               │
│                                                 │
│  Option 1: REST (Synchronous)                   │
│  HR.API ──HTTP GET──► Accounting.API           │
│                                                 │
│  Option 2: Events (Asynchronous)                │
│  HR.API ──Publish "EmployeeHired"──► Event Bus │
│           ▲                                     │
│           └──Subscribe── Accounting.API         │
│           └──Subscribe── Payroll.API            │
│           └──Subscribe── Benefits.API           │
└─────────────────────────────────────────────────┘
```

### Clean Architecture (Per Service)
```
Each Microservice follows Clean Architecture:

┌─────────────────────────────────────────┐
│      Presentation (X.API, X.Web)        │
│  - Controllers                          │
│  - Blazor Components                    │
│  - Mobile Contracts                     │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│      Application (X.Application)        │
│  - Commands/Queries (CQRS)              │
│  - DTOs                                 │
│  - Interfaces                           │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│         Domain (X.Domain)               │
│  - Entities                             │
│  - Value Objects                        │
│  - Domain Services                      │
│  - Domain Events                        │
└─────────────────────────────────────────┘
                ▲
                │
┌───────────────┴─────────────────────────┐
│    Infrastructure (X.Infrastructure)    │
│  - EF Core DbContext                    │
│  - Repositories                         │
│  - Event Publishers                     │
│  - External Services                    │
└─────────────────────────────────────────┘
```

---

## 🐳 Docker & Deployment Architecture

### Current Docker Compose (Monolithic)
```yaml
services:
  web:       # Blazor Web (Port 3000)
  admin:     # Admin Portal (Port 8080)
  backend:   # Monolithic API (Port 5000)
  db:        # SQL Server (Internal)
```

### Target Docker Compose (Microservices)
```yaml
services:
  # Platform Services
  api-gateway:      # Port 5000 (Nginx/Ocelot)
  identity-api:     # Port 5001 (Auth service)
  platform-api:     # Port 5002 (Platform services)

  # Business Microservices
  hr-api:           # Port 5010
  hr-db:            # SQL Server (HR schema)

  accounting-api:   # Port 5020
  accounting-db:    # SQL Server (Accounting schema)

  inventory-api:    # Port 5030
  inventory-db:     # SQL Server (Inventory schema)

  # Frontend
  web-shell:        # Port 3000 (Blazor shell + modules)
  admin:            # Port 8080 (Admin portal)

  # Infrastructure
  rabbitmq:         # Message bus
  redis:            # Distributed cache
  seq:              # Centralized logging (dev)
```

### Service Deployment Pattern

**Each service gets:**
1. **Dockerfile** for containerization
2. **Independent deployment** to AWS ECS/EKS
3. **Own database** (or schema)
4. **Health check endpoint** (`/health`)
5. **Metrics endpoint** (`/metrics`)
6. **Swagger documentation** (`/swagger`)

**Example: HR Service Deployment**
```
HR.API (Docker Container)
├── Health: http://hr-api:5010/health
├── Metrics: http://hr-api:5010/metrics
├── Swagger: http://hr-api:5010/swagger
└── Database: hr-db:1433 (or RDS instance)
```

### AWS Deployment Strategy

**Current:**
- Single EC2 instance running Docker Compose
- Single RDS instance (multi-tenant databases)

**Target (Cost-Optimized Microservices):**

#### 💰 Budget Constraint: $100/month for EC2

**Recommended Setup (3 EC2 Instances):**

```
┌─────────────────────────────────────────────────────────┐
│                    AWS Cloud                            │
│                                                         │
│  ┌──────────────────────────────────────┐              │
│  │  Application Load Balancer (ALB)     │              │
│  │  $16.20/mo (720 hours)               │              │
│  └──────┬───────────────┬───────────────┘              │
│         │               │                               │
│  ┌──────▼─────────┐  ┌──▼─────────────────┐           │
│  │   EC2 Small    │  │   EC2 Micro        │           │
│  │   (t3.small)   │  │   (t3.micro)       │           │
│  │   $15.18/mo    │  │   $7.59/mo         │           │
│  ├────────────────┤  ├────────────────────┤           │
│  │ High-Traffic   │  │ Low-Traffic        │           │
│  │ Services:      │  │ Services:          │           │
│  │                │  │                    │           │
│  │ • HR.API       │  │ • Assets.API       │           │
│  │ • Accounting   │  │ • Facilities       │           │
│  │ • Inventory    │  │ • Marketing        │           │
│  │ • Timekeeping  │  │ • Support          │           │
│  │ • Sales        │  │ • Projects         │           │
│  │                │  │                    │           │
│  │ Docker Compose │  │ Docker Compose     │           │
│  │ (5-8 services) │  │ (5-10 services)    │           │
│  └────────┬───────┘  └────────┬───────────┘           │
│           │                   │                        │
│  ┌────────▼───────────────────▼───────┐               │
│  │   EC2 Medium (t3.medium) - CURRENT │               │
│  │   $30.37/mo                         │               │
│  ├─────────────────────────────────────┤               │
│  │ Platform Services:                  │               │
│  │                                     │               │
│  │ • API Gateway (Nginx/YARP)          │               │
│  │ • Identity Service (Auth)           │               │
│  │ • Platform API                      │               │
│  │ • Admin Portal                      │               │
│  │ • Web Shell (Blazor)                │               │
│  │ • Message Bus (RabbitMQ)            │               │
│  │ • Redis Cache                       │               │
│  │                                     │               │
│  │ Docker Compose (7-10 containers)    │               │
│  └──────────────┬──────────────────────┘               │
│                 │                                      │
│  ┌──────────────▼──────────────────────┐              │
│  │   RDS SQL Server (db.t3.small)      │              │
│  │   $24.89/mo (Single-AZ)             │              │
│  │   Multi-tenant databases            │              │
│  └─────────────────────────────────────┘              │
│                                                        │
│  ┌─────────────────────────────────────┐              │
│  │  Other Services (minimal cost):     │              │
│  │  • S3 (static assets) - ~$1-5/mo    │              │
│  │  • CloudWatch (logs) - ~$5-10/mo    │              │
│  │  • Secrets Manager - ~$0.40/mo      │              │
│  └─────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────┘

💵 Total EC2 Cost: ~$53/mo (well under $100 budget!)
💵 Total AWS Cost: ~$95-110/mo (including RDS, ALB, misc)
```

#### 📊 Cost Breakdown

| Service | Type | Monthly Cost | Purpose |
|---------|------|--------------|---------|
| **EC2 (Current)** | t3.medium | $30.37 | Platform services, gateway, web |
| **EC2 (High-traffic)** | t3.small | $15.18 | Core business services |
| **EC2 (Low-traffic)** | t3.micro | $7.59 | Rarely used modules |
| **RDS** | db.t3.small | $24.89 | SQL Server database |
| **ALB** | Load Balancer | $16.20 | Traffic routing |
| **S3** | Storage | $1-5 | Static assets |
| **CloudWatch** | Monitoring | $5-10 | Logs & metrics |
| **Secrets Manager** | Config | $0.40 | Credentials |
| **Data Transfer** | Network | $5-10 | Bandwidth |
| **TOTAL** | | **~$100-105** | |

#### 🎯 Service Grouping Strategy

**EC2 Medium (Current/Platform) - t3.medium ($30/mo):**
- **Must stay together:** Gateway, Auth, Admin, Web Shell, Redis, RabbitMQ
- **Why:** High availability requirements, shared by all services
- **Resources:** 2 vCPU, 4 GB RAM
- **Docker containers:** ~7-10

**EC2 Small (High-Traffic) - t3.small ($15/mo):**
- HR (employees used daily)
- Accounting (financial transactions)
- Inventory (stock checks)
- Timekeeping (clock in/out)
- Sales/CRM (customer interactions)
- **Resources:** 2 vCPU, 2 GB RAM
- **Docker containers:** ~5-8

**EC2 Micro (Low-Traffic) - t3.micro ($8/mo):**
- Assets Management (occasional use)
- Facilities (maintenance scheduling)
- Marketing (campaigns)
- Support/Helpdesk (tickets)
- Projects (project tracking)
- Field Services
- **Resources:** 2 vCPU, 1 GB RAM
- **Docker containers:** ~5-10

#### 🔄 Docker Compose Per Instance

**Platform Instance (t3.medium):**
```yaml
# docker-compose.platform.yml
services:
  api-gateway:      # Nginx or YARP
  identity-api:     # Auth service
  platform-api:     # Platform services
  admin-portal:     # Admin UI
  web-shell:        # Blazor shell
  rabbitmq:         # Message bus
  redis:            # Cache
```

**High-Traffic Instance (t3.small):**
```yaml
# docker-compose.hightraffic.yml
services:
  hr-api:
  accounting-api:
  inventory-api:
  timekeeping-api:
  sales-api:
```

**Low-Traffic Instance (t3.micro):**
```yaml
# docker-compose.lowtraffic.yml
services:
  assets-api:
  facilities-api:
  marketing-api:
  support-api:
  projects-api:
  fieldservice-api:
```

#### 📈 Scaling Strategy (When to Add More)

**If you exceed $100 budget, scale in this order:**

1. **First:** Add t3.micro ($8) for 5-10 more low-traffic services
2. **Second:** Upgrade high-traffic from t3.small → t3.medium ($15 more)
3. **Third:** Split high-traffic services across 2x t3.small
4. **Fourth:** Add dedicated instance for specific heavy service (e.g., reporting)

**When to separate a service to its own instance:**
- Service uses >50% CPU consistently
- Service causes memory pressure on shared instance
- Service has critical SLA requirements
- Service needs independent scaling

#### 🎨 Deployment Architecture Visual

```
Internet
   │
   ▼
┌──────────────────┐
│   Route 53 DNS   │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│  ALB (Port 443)  │   Routes by path:
│  $16/mo          │   /api/hr/*       → EC2 Small
└────────┬─────────┘   /api/assets/*   → EC2 Micro
         │             /api/auth/*     → EC2 Medium
         │             /admin/*        → EC2 Medium
    ┌────┴────┬─────────────┬──────────────┐
    │         │             │              │
┌───▼────┐ ┌──▼──────┐  ┌──▼────────┐  ┌──▼────────┐
│ Medium │ │ Small   │  │ Micro     │  │ RDS       │
│ $30/mo │ │ $15/mo  │  │ $8/mo     │  │ $25/mo    │
│        │ │         │  │           │  │           │
│ 7-10   │ │ 5-8     │  │ 5-10      │  │ All DBs   │
│ svcs   │ │ svcs    │  │ svcs      │  │           │
└────────┘ └─────────┘  └───────────┘  └───────────┘
```

#### 💡 Cost Optimization Tips

1. **Use Reserved Instances** - Save 30-40% if committed for 1 year
2. **Stop dev instances at night** - Cut costs in half
3. **Use spot instances** for non-critical services - Save 70%
4. **CloudWatch Log Retention** - Set to 7 days (not default 30)
5. **S3 Lifecycle Policies** - Move old files to Glacier
6. **Right-size RDS** - Start small, scale up only if needed
7. **Use CloudFront CDN** - Reduce data transfer costs
8. **Enable detailed monitoring only for critical services**

#### 🚨 Cost Alerts

Set up AWS Budget alerts:
- **$50** - Warning (50% of budget)
- **$80** - Urgent (80% of budget)
- **$100** - Critical (budget exceeded)

#### 📊 Monthly Cost Monitoring

**Track these metrics in Admin Portal:**

1. **Per-Instance Utilization**
   - CPU usage (should be <70% average)
   - Memory usage (should be <80%)
   - Network bandwidth
   - Disk I/O

2. **Per-Service Metrics**
   - Request count (requests/hour)
   - Response time (avg latency)
   - Error rate
   - Resource consumption

3. **Cost per Service** (estimate)
   - Calculate: `(Service CPU% / Instance CPU%) × Instance Cost`
   - Example: HR uses 30% of t3.small → ~$4.55/mo allocated

**Admin Dashboard Should Show:**
```
┌─────────────────────────────────────────────┐
│  AWS Cost Dashboard                         │
├─────────────────────────────────────────────┤
│  Current Month: $87.50 / $100 (87%)        │
│  Projected: $95.00                          │
│                                             │
│  EC2 Instances:                             │
│  ✅ Platform (t3.medium)    $30.37  [70%]  │
│  ✅ High-Traffic (t3.small) $15.18  [65%]  │
│  ✅ Low-Traffic (t3.micro)  $7.59   [45%]  │
│                                             │
│  Top 5 Services by Resource Usage:         │
│  1. Accounting.API    25% CPU  [Move?]     │
│  2. HR.API            20% CPU              │
│  3. Inventory.API     18% CPU              │
│  4. Timekeeping.API   15% CPU              │
│  5. Sales.API         12% CPU              │
│                                             │
│  ⚠️ Recommendations:                        │
│  • Consider moving Accounting to dedicated  │
│    instance if usage exceeds 30%           │
└─────────────────────────────────────────────┘
```

**Implementation Note for AI:**
- Add cost tracking to `frontend/BusinessAsUsual.Admin/Areas/Admin/Controllers/MonitoringController.cs`
- Create `CostDashboardViewModel.cs` in Models
- Query CloudWatch for EC2 metrics
- Calculate per-service cost allocation
- Display in Admin dashboard

---

## 💾 Database Strategy (Cost Optimization)

### Current: Single RDS Instance with Multi-Tenant Databases

**RDS db.t3.small ($24.89/mo):**
- All tenant databases on one instance
- Each service has own schema/database
- Connection string routing based on service

**Example:**
```
RDS Instance: business-as-usual.abc123.us-east-1.rds.amazonaws.com
├── master (metadata)
├── tenant_companyA_hr
├── tenant_companyA_accounting
├── tenant_companyA_inventory
├── tenant_companyB_hr
├── tenant_companyB_accounting
└── tenant_companyB_inventory
```

### Database Per Service (Within Budget)

**Don't create separate RDS instances** - too expensive!
Instead: **Logical separation on same RDS instance**

```csharp
// Each service connects to same RDS, different database
// HR.API appsettings.json
{
  "ConnectionStrings": {
    "Default": "Server=rds-host;Database=tenant_{tenantId}_hr;..."
  }
}

// Accounting.API appsettings.json
{
  "ConnectionStrings": {
    "Default": "Server=rds-host;Database=tenant_{tenantId}_accounting;..."
  }
}
```

**Benefits:**
- True data isolation per service
- Independent schema evolution
- Can extract to separate RDS later
- Single RDS instance cost ($25/mo)

**When to add second RDS instance?**
- When first instance reaches 80% CPU
- When storage exceeds 100GB
- When specific service needs high IOPS
- **Cost impact:** +$25/mo for second db.t3.small

### CI/CD Pipeline (GitHub Actions)

**Current:** Single pipeline builds all projects  
**Target:** Independent pipelines per service (deploy to specific EC2)

```yaml
# .github/workflows/hr-service.yml
name: HR Service CI/CD

on:
  push:
    paths:
      - 'services/HR/**'

env:
  TARGET_INSTANCE: 'high-traffic'  # or 'low-traffic' or 'platform'

jobs:
  build:
    - Build HR.API
    - Run HR.Tests
    - Build HR.Web
    - Build Docker image
    - Push to Docker Hub or ECR

  deploy:
    - SSH to correct EC2 instance ($TARGET_INSTANCE)
    - Pull latest images
    - Update docker-compose file
    - Restart service containers
    - Run smoke tests
    - Notify Slack/Teams
```

#### 📊 Service Deployment Matrix

| Service | Target Instance | Compose File | Port |
|---------|----------------|--------------|------|
| API Gateway | platform | docker-compose.platform.yml | 5000 |
| Identity | platform | docker-compose.platform.yml | 5001 |
| Admin Portal | platform | docker-compose.platform.yml | 8080 |
| Web Shell | platform | docker-compose.platform.yml | 3000 |
| HR.API | high-traffic | docker-compose.hightraffic.yml | 5010 |
| Accounting.API | high-traffic | docker-compose.hightraffic.yml | 5020 |
| Inventory.API | high-traffic | docker-compose.hightraffic.yml | 5030 |
| Assets.API | low-traffic | docker-compose.lowtraffic.yml | 5100 |
| Marketing.API | low-traffic | docker-compose.lowtraffic.yml | 5110 |

#### 🔄 Moving Services Between Instances

**When a low-traffic service becomes high-traffic:**

1. **Update GitHub Actions workflow** - change `TARGET_INSTANCE`
2. **Update ALB routing rules** - point to new instance IP
3. **Deploy to new instance** - run pipeline
4. **Test thoroughly** - smoke tests
5. **Remove from old instance** - cleanup
6. **Update documentation** - deployment matrix above

**Example: Moving Assets.API from Micro to Small**
```bash
# 1. SSH to high-traffic instance
ssh ec2-user@high-traffic-instance

# 2. Update docker-compose.hightraffic.yml
# Add assets-api service

# 3. Pull and start
docker-compose -f docker-compose.hightraffic.yml pull assets-api
docker-compose -f docker-compose.hightraffic.yml up -d assets-api

# 4. Update ALB target group (AWS Console or CLI)

# 5. Test
curl https://api.businessasusual.com/api/assets/health

# 6. Remove from low-traffic instance
ssh ec2-user@low-traffic-instance
docker-compose -f docker-compose.lowtraffic.yml stop assets-api
docker-compose -f docker-compose.lowtraffic.yml rm assets-api
```

---

---

## 🎯 Implementation Roadmap: Transitioning to Microservices

### Phase 1: Foundation & First Service (Current Focus) 🎯

**Goal:** Extract HR module as first microservice with UI injection

#### Step 1.1: Create Service Structure
```bash
mkdir -p services/HR/HR.API
mkdir -p services/HR/HR.Application
mkdir -p services/HR/HR.Domain
mkdir -p services/HR/HR.Infrastructure
mkdir -p services/HR/HR.Web
mkdir -p services/HR/HR.Contracts
mkdir -p services/HR/HR.Tests
```

#### Step 1.2: Move Domain Models
- Move `backend/BusinessAsUsual.Domain/Modules/HR.Domain/` → `services/HR/HR.Domain/`
- Define entities: Employee, Department, Position, TimeOff, etc.

#### Step 1.3: Build HR.API
- Create `Program.cs` with minimal API
- Create controllers: EmployeesController, DepartmentsController, UISpecController
- Add Swagger documentation
- Configure health checks
- Add metrics middleware

#### Step 1.4: Create HR.Web (Blazor Components)
- Build reusable components:
  - `EmployeeCard.razor`
  - `EmployeeList.razor`
  - `EmployeeDetail.razor`
  - `DepartmentSelector.razor`
- Build full pages:
  - `Employees.razor` (route: `/hr/employees`)
  - `EmployeeDetail.razor` (route: `/hr/employees/{id}`)
  - `Dashboard.razor` (route: `/hr`)
- Add `ModuleMetadata.cs` for route registration

#### Step 1.5: Create HR.Contracts (Mobile UI Specs)
- `UIModels/EmployeeViewModel.cs`
- `UIModels/DepartmentViewModel.cs`
- `Specifications/EmployeeListSpec.cs`
- `Specifications/EmployeeFormSpec.cs`
- `Navigation/HRNavigationMap.cs`

#### Step 1.6: Implement Component Injection in Main Web App
- Add module discovery service to `BusinessAsUsual.Web`
- Implement assembly scanning for `*.Web.dll`
- Register discovered components dynamically
- Update MainLayout to show injected modules
- Test local development with HR.Web loaded

#### Step 1.7: Deploy HR Service
- Create `Dockerfile` for HR.API
- Add to `docker-compose.yml`
- Configure separate database (or schema)
- Test end-to-end flow

**Success Criteria:**
- ✅ HR service runs independently on port 5010
- ✅ HR.Web components load in BusinessAsUsual.Web shell
- ✅ Mobile can fetch HR UI specs from `/api/hr/ui/`
- ✅ HR data operations work through HR.API
- ✅ Tests pass for HR service

---

### Phase 2: Extract Additional Core Services

**Services to Extract:**
1. **Accounting Service**
   - Financial transactions, GL, AP, AR
   - Port: 5020

2. **Inventory Service**
   - Products, stock, warehouses
   - Port: 5030

3. **Timekeeping Service**
   - Time entries, schedules, approvals
   - Port: 5040

**For each service:**
- Follow Phase 1 pattern
- Add service-to-service communication (REST + Events)
- Update Admin portal to monitor service health

---

### Phase 3: Platform Services & API Gateway

#### 3.1: Extract Platform Services
- **Identity Service** (Auth, users, roles) - Port 5001
- **Notification Service** (Email, SMS, push) - Port 5002
- **Audit Log Service** (System events) - Port 5003

#### 3.2: Implement API Gateway
- Use Ocelot or YARP (Yet Another Reverse Proxy)
- Route requests to services
- Handle authentication at gateway
- Rate limiting & throttling

#### 3.3: Service Discovery
- Use Consul or Azure Service Fabric
- Services register themselves
- Gateway discovers services dynamically

---

### Phase 4: Event-Driven Architecture

#### 4.1: Implement Message Bus
- Use RabbitMQ, Azure Service Bus, or AWS SNS/SQS
- Define event contracts

#### 4.2: Event Publishing
Example: HR Service publishes events
```csharp
// When employee is hired
await eventBus.PublishAsync(new EmployeeHiredEvent
{
    EmployeeId = employee.Id,
    HireDate = employee.HireDate,
    Department = employee.Department,
    CostCenter = employee.CostCenter
});
```

#### 4.3: Event Subscriptions
Other services react to events
```csharp
// Accounting service subscribes to EmployeeHiredEvent
public class EmployeeHiredEventHandler : IEventHandler<EmployeeHiredEvent>
{
    public async Task Handle(EmployeeHiredEvent @event)
    {
        // Create cost center allocation
        // Set up payroll account
        // Initialize expense tracking
    }
}
```

---

### Phase 5: Monitoring & Observability

#### 5.1: Distributed Tracing
- Implement OpenTelemetry
- Trace requests across services
- Visualize with Jaeger or Application Insights

#### 5.2: Centralized Logging
- Aggregate logs from all services
- Use Seq (local), CloudWatch (AWS), or ELK stack
- Correlation IDs across requests

#### 5.3: Metrics & Dashboards
- Service-level metrics (requests, errors, latency)
- Business metrics (employees added, invoices processed)
- Grafana dashboards

---

### Phase 6: Mobile Enhancement

#### 6.1: Enhance Mobile Contracts
- Add form validation rules to specs
- Define conditional logic (show field X if Y is selected)
- Add localization support in specs

#### 6.2: Offline Sync Strategy
- Define sync protocols
- Conflict resolution rules
- Background sync service

#### 6.3: Push Notification Integration
- Define notification contracts
- Implement per-module notification preferences
- Test cross-platform (Android/iOS)

---

## 📱 Mobile Architecture Deep Dive

### Relationship Between Web Solution and Mobile Apps

**Separate Repositories:**
- **BusinessAsUsual** (this repo) - Web + API + Backend
- **BusinessAsUsual-Android** (separate repo) - Native Android app (Kotlin + Jetpack Compose)
- **BusinessAsUsual-iOS** (separate repo) - Native iOS app (Swift + SwiftUI)

**Architecture:** Both mobile apps follow **MVVM Clean Architecture** pattern

📚 **Detailed Mobile Architecture Guide:** See [`docs/MOBILE_ARCHITECTURE.md`](MOBILE_ARCHITECTURE.md)

### Mobile Architecture Summary

Both iOS and Android implement **MVVM (Model-View-ViewModel) Clean Architecture:**

```
Presentation Layer (Views + ViewModels)
         ↓
Domain Layer (Use Cases + Entities + Repository Interfaces)
         ↓
Data Layer (Repository Implementations + API + Local DB)
```

**Key Principles:**
- **Separation of Concerns** - Strict layer boundaries
- **Dependency Inversion** - Domain doesn't depend on Data
- **Offline-First** - Local database as source of truth
- **UI Contracts** - Backend provides UI specifications
- **Testability** - Clean separation enables easy testing

### Current iOS Implementation Issues 🚨

**Current State (BusinessAsUsual-iOS repo):**
- ❌ No proper architecture - just basic SwiftUI views
- ❌ No MVVM pattern - all logic in views
- ❌ No domain layer - no business logic separation
- ❌ No data layer - no API integration, no local database
- ❌ No dependency injection - hard-coded dependencies
- ❌ No navigation pattern - basic SwiftUI navigation
- ❌ Only theme system implemented (BAUTheme.swift)

**Files Currently in iOS Repo:**
```
BusinessAsUsualiOS/
├── BusinessAsUsualiOSApp.swift      # App entry (basic)
├── ContentView.swift                # Main view (placeholder)
├── BAUTheme.swift                   # ✅ Theme system (good!)
├── BAUTopBar.swift                  # Basic top bar component
├── BAUScreenShell.swift             # Screen shell
└── BAUBreadcrumbBar.swift           # Breadcrumb component
```

**What Needs to Be Done:**
1. ✅ Keep theme system (BAUTheme.swift is actually well-designed)
2. ❌ Restructure entire project following Clean Architecture
3. ❌ Add proper folder structure (see MOBILE_ARCHITECTURE.md)
4. ❌ Implement MVVM pattern with ViewModels
5. ❌ Add domain layer (entities, use cases, repository interfaces)
6. ❌ Add data layer (API client, local database, repositories)
7. ❌ Implement dependency injection container
8. ❌ Add navigation coordinator pattern
9. ❌ Integrate with backend API
10. ❌ Implement UI spec consumption from API

**Good News:** The theme system you created is actually solid! Keep that pattern.

**Action Plan:**
- See detailed refactoring guide in [`docs/MOBILE_ARCHITECTURE.md`](MOBILE_ARCHITECTURE.md)
- Follow the iOS project structure defined there
- Start with HR module as proof of concept
- Implement offline-first data sync

### Mobile App Architecture

#### **How Mobile Apps Consume Services**

```
┌────────────────────────────────────────────────┐
│          Android/iOS App                       │
│                                                │
│  1. App Startup                                │
│     ├─ GET /api/modules/available              │
│     │  Response: ["HR", "Accounting", "Inv"]   │
│     │                                           │
│     └─ For each module:                        │
│        GET /api/{module}/ui-spec               │
│        Response: UI contract (JSON)            │
│                                                │
│  2. User navigates to "Employees"              │
│     ├─ Load HR.Contracts.EmployeeListSpec      │
│     │  - Title, columns, filters, actions      │
│     │                                           │
│     ├─ Render native Android UI                │
│     │  (RecyclerView with spec-defined cols)   │
│     │                                           │
│     └─ GET /api/hr/employees                   │
│        Response: Employee data (JSON)          │
│                                                │
│  3. User adds new employee                     │
│     ├─ GET /api/hr/employees/form-spec         │
│     │  Response: Form fields, validation rules │
│     │                                           │
│     ├─ Render native form (Material Design)    │
│     │                                           │
│     └─ POST /api/hr/employees                  │
│        Response: Created employee              │
└────────────────────────────────────────────────┘
```

### UI Contract Pattern (Detailed)

**Backend Service Defines UI Contract:**
```csharp
// services/HR/HR.Contracts/UIModels/EmployeeViewModel.cs
public class EmployeeViewModel
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; }
    public string PhotoUrl { get; set; }
    public string Department { get; set; }
    public string JobTitle { get; set; }
    public DateTime HireDate { get; set; }
}

// services/HR/HR.Contracts/Specifications/EmployeeListSpec.cs
public class EmployeeListSpec : IUISpecification
{
    public string Title => "Employees";
    public string Icon => "person_outline";
    public bool Searchable => true;
    public string SearchPlaceholder => "Search by name or email...";

    public List<ColumnDefinition> Columns => new()
    {
        new("fullName", "Name", sortable: true, filterable: true),
        new("department", "Department", sortable: true, filterable: true),
        new("jobTitle", "Job Title", sortable: false),
        new("email", "Email", sortable: false)
    };

    public List<ActionDefinition> Actions => new()
    {
        new("add", "Add Employee", "add_circle", "/employees/new"),
        new("export", "Export", "download", null, actionType: "download")
    };

    public PaginationConfig Pagination => new(pageSize: 25, showSizeOptions: true);
}

// services/HR/HR.API/Controllers/UISpecController.cs
[ApiController]
[Route("api/hr/ui")]
public class UISpecController : ControllerBase
{
    [HttpGet("employees/list")]
    public IActionResult GetEmployeeListSpec()
    {
        return Ok(new EmployeeListSpec());
    }

    [HttpGet("employees/form")]
    public IActionResult GetEmployeeFormSpec()
    {
        return Ok(new EmployeeFormSpec());
    }
}
```

**Mobile App Consumes Contract:**
```kotlin
// Android: MainActivity.kt
class EmployeeListActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        lifecycleScope.launch {
            // Fetch UI spec from API
            val spec = api.getEmployeeListSpec()

            // Configure RecyclerView based on spec
            toolbar.title = spec.title
            setupSearch(spec.searchable, spec.searchPlaceholder)

            // Configure columns
            val adapter = DynamicTableAdapter(spec.columns)
            recyclerView.adapter = adapter

            // Configure actions
            spec.actions.forEach { action ->
                when (action.actionType) {
                    "navigation" -> addFab(action.label, action.icon) {
                        navigate(action.route)
                    }
                    "download" -> addMenuItem(action.label, action.icon) {
                        exportData()
                    }
                }
            }

            // Fetch data
            val employees = api.getEmployees()
            adapter.submitList(employees)
        }
    }
}
```

### Why This Approach?

**Benefits:**
1. **Backend controls UI** - Changes to UI structure don't require mobile app updates
2. **Consistent UX** - All platforms follow same patterns
3. **Dynamic forms** - Form fields can change without code updates
4. **Feature toggles** - Backend can enable/disable features per tenant
5. **A/B testing** - Different UI specs for different users

**Example Use Cases:**
- Add a new column to employee list → Just update spec, no app update needed
- Change field validation → Update spec, all apps comply
- Hide feature for specific tenant → Spec excludes it, app doesn't show it

### Mobile Data Flow

```
┌─────────────────────────────────────────────┐
│       Android/iOS App (Local)               │
│                                             │
│  ┌─────────────────────────────────┐        │
│  │  Local SQLite Cache             │        │
│  │  - Offline employee data         │        │
│  │  - UI specs (cached)            │        │
│  └─────────────────────────────────┘        │
│              ▲                              │
│              │ Sync                         │
│              ▼                              │
└──────────────┼──────────────────────────────┘
               │
          Internet
               │
┌──────────────▼──────────────────────────────┐
│         Cloud (AWS)                         │
│                                             │
│  ┌─────────────────────────────────┐        │
│  │    API Gateway / Load Balancer  │        │
│  └────────┬────────────────────────┘        │
│           │                                 │
│  ┌────────▼────────┐                        │
│  │   HR.API        │                        │
│  │   - REST API    │                        │
│  │   - UI Specs    │                        │
│  └────────┬────────┘                        │
│           │                                 │
│  ┌────────▼────────┐                        │
│  │    HR Database  │                        │
│  └─────────────────┘                        │
└─────────────────────────────────────────────┘
```

### Mobile App Features

**Core Capabilities:**
1. **Offline-first** - Local SQLite cache, sync when online
2. **Push notifications** - Firebase Cloud Messaging
3. **Biometric auth** - Fingerprint/Face unlock
4. **Camera integration** - Photo uploads (employee photos, receipts)
5. **QR code scanning** - Asset tracking, inventory
6. **Geolocation** - Field service, time tracking

**Module Support:**
- HR - Employee directory, time off requests
- Time Tracking - Clock in/out, timesheet entry
- Inventory - Barcode scanning, stock checks
- Field Service - Work orders, GPS tracking
- Expense Management - Receipt capture, approval workflow

---

---

## 🔐 Security & Authentication Architecture

### Current Implementation (Monolithic)
- JWT-based authentication
- HTTPS enforcement
- CORS configuration
- Rate limiting

### Target Implementation (Microservices)

#### Authentication Flow
```
1. User logs in → Identity Service (Port 5001)
2. Identity Service validates credentials
3. Returns JWT with claims (userId, tenantId, roles)
4. User includes JWT in requests to services
5. Each service validates JWT (shared secret or public key)
```

#### API Gateway Security
```
┌──────────────────────────────────────────┐
│         API Gateway (Port 443)           │
│  - SSL Termination                       │
│  - JWT Validation                        │
│  - Rate Limiting                         │
│  - IP Whitelisting                       │
└────────────┬─────────────────────────────┘
             │ Trusted Internal Network
             │ (JWT already validated)
             │
    ┌────────┼────────┬────────────┐
    │        │        │            │
┌───▼───┐ ┌─▼──┐ ┌───▼───┐  ┌────▼────┐
│HR.API │ │Acct│ │Inv.API│  │Time.API │
│       │ │.API│ │       │  │         │
└───────┘ └────┘ └───────┘  └─────────┘
```

#### Per-Service Authorization
Each service validates permissions for its domain:
```csharp
// HR.API validates HR-specific permissions
[Authorize(Policy = "HR.ViewEmployees")]
public async Task<IActionResult> GetEmployees()
{
    // User has HR.ViewEmployees permission
}
```

### Multi-Tenant Security

**Tenant Isolation:**
1. **Database Per Tenant** - Each company has separate database
2. **Tenant Context Middleware** - Extracts tenant from JWT
3. **Connection String Routing** - Routes to correct database
4. **Cross-Tenant Prevention** - Impossible to access other tenant's data

**Provisioning Flow:**
1. Admin creates company in Admin Portal
2. Request → Platform API `/api/provisioning/provision`
3. `ProvisioningService` creates new database
4. Schema applied from template
5. Tenant metadata stored
6. Company assigned unique identifier
7. Users receive JWT with `tenantId` claim

---

## 📊 Monitoring & Observability (Enhanced)

### Current State
- Serilog structured logging
- CloudWatch metrics (production)
- Health checks (memory, disk)

### Target State: Full Observability

#### 1. Distributed Tracing
**Tool:** OpenTelemetry + Jaeger/Application Insights

**Trace Example:**
```
Request: GET /hr/employees
│
├─ API Gateway (2ms)
│  └─ JWT Validation (1ms)
│
├─ HR.API Controller (5ms)
│  ├─ HR.Application.GetEmployeesQuery (15ms)
│  │  ├─ HR.Infrastructure.Repository (40ms)
│  │  │  └─ SQL Query (38ms)
│  │  └─ Mapping to DTOs (2ms)
│  └─ Response serialization (3ms)
│
└─ Total: 65ms
```

Benefits:
- See exactly where time is spent
- Identify slow services
- Trace errors across services

#### 2. Centralized Logging
**Tools:**
- **Development:** Seq (Docker container)
- **Production:** AWS CloudWatch or ELK Stack

**Log Aggregation:**
```
All Services ──► Log Aggregator ──► Search/Alert

HR.API logs:
{
  "timestamp": "2025-01-15T10:30:00Z",
  "level": "Information",
  "message": "Employee created",
  "correlationId": "abc-123",
  "service": "HR.API",
  "employeeId": "EMP001",
  "userId": "user123",
  "tenantId": "tenant456"
}
```

#### 3. Metrics & Dashboards
**Service-Level Metrics:**
- Request rate (requests/second)
- Error rate (errors/total requests)
- Latency (p50, p95, p99)
- Saturation (CPU, memory, disk)

**Business Metrics:**
- Employees added today
- Invoices processed this hour
- Time entries submitted
- Module usage by tenant

**Dashboard Example (Grafana):**
```
┌─────────────────────────────────────────┐
│  Service Health Overview                │
├─────────────────────────────────────────┤
│  HR.API:        ✅ Healthy (5010)       │
│  Accounting:    ✅ Healthy (5020)       │
│  Inventory:     ⚠️  Slow (5030)         │
│  Timekeeping:   ✅ Healthy (5040)       │
├─────────────────────────────────────────┤
│  Request Rate: 1,234 req/min           │
│  Error Rate:   0.02%                    │
│  Avg Latency:  120ms                    │
└─────────────────────────────────────────┘
```

#### 4. Alerting
**Alert Rules:**
- Error rate > 1% for 5 minutes → Page on-call
- Latency p95 > 1s → Slack notification
- Service down → Immediate page
- Disk space < 10% → Email alert

---

## 🧪 Testing Strategy

### Current State
- Unit tests in `BusinessAsUsual.Tests`

### Target State: Comprehensive Testing

#### 1. Unit Tests (Per Service)
```
services/HR/HR.Tests/
├── Unit/
│   ├── Domain/
│   │   └── EmployeeTests.cs
│   ├── Application/
│   │   ├── CreateEmployeeCommandTests.cs
│   │   └── GetEmployeesQueryTests.cs
│   └── Infrastructure/
│       └── EmployeeRepositoryTests.cs
```

#### 2. Integration Tests (API)
```csharp
[TestClass]
public class EmployeeApiTests
{
    [TestMethod]
    public async Task GetEmployees_ReturnsEmployeeList()
    {
        // Arrange
        using var client = CreateTestClient();
        await SeedTestData();

        // Act
        var response = await client.GetAsync("/api/employees");

        // Assert
        response.EnsureSuccessStatusCode();
        var employees = await response.Content.ReadAsAsync<List<EmployeeDto>>();
        Assert.AreEqual(5, employees.Count);
    }
}
```

#### 3. Component Tests (Blazor)
```csharp
[TestClass]
public class EmployeeListComponentTests
{
    [TestMethod]
    public void EmployeeList_RendersCorrectly()
    {
        // Arrange
        using var ctx = new TestContext();
        var employees = GetTestEmployees();

        // Act
        var cut = ctx.RenderComponent<EmployeeList>(parameters => 
            parameters.Add(p => p.Employees, employees));

        // Assert
        cut.MarkupMatches(@"
            <table>
                <tr><td>John Doe</td><td>Engineering</td></tr>
                <tr><td>Jane Smith</td><td>Sales</td></tr>
            </table>
        ");
    }
}
```

#### 4. End-to-End Tests (Mobile/Web)
```csharp
// Use Playwright for web E2E
[TestMethod]
public async Task UserCanAddEmployee()
{
    // Arrange
    await using var browser = await Playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();

    // Act
    await page.GotoAsync("https://localhost:3000/hr/employees");
    await page.ClickAsync("text=Add Employee");
    await page.FillAsync("#firstName", "John");
    await page.FillAsync("#lastName", "Doe");
    await page.ClickAsync("button:has-text('Save')");

    // Assert
    await page.WaitForSelectorAsync("text=Employee added successfully");
}
```

#### 5. Contract Tests (API)
Ensure mobile apps and API stay in sync:
```csharp
[TestMethod]
public void EmployeeViewModel_MatchesContract()
{
    var employee = new EmployeeViewModel
    {
        Id = "123",
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com"
    };

    var json = JsonSerializer.Serialize(employee);

    // Verify JSON structure matches what mobile expects
    var doc = JsonDocument.Parse(json);
    Assert.IsTrue(doc.RootElement.TryGetProperty("id", out _));
    Assert.IsTrue(doc.RootElement.TryGetProperty("firstName", out _));
}
```

---

---

## 🛠️ Development Guide (For GitHub Copilot AI)

### When User Says: "Add a new module"

**Example:** "Add a new Payroll module"

**Steps:**
1. **Create service structure:**
   ```bash
   mkdir -p services/Payroll/Payroll.API
   mkdir -p services/Payroll/Payroll.Application
   mkdir -p services/Payroll/Payroll.Domain
   mkdir -p services/Payroll/Payroll.Infrastructure
   mkdir -p services/Payroll/Payroll.Web
   mkdir -p services/Payroll/Payroll.Contracts
   ```

2. **Add to BusinessAsUsual.Core/Modules/ModuleCatalog.cs:**
   ```csharp
   new("HR_Payroll", "Payroll", "Payroll Management", new []
   {
       new SubmoduleDefinition("PayrollRuns", "Payroll Runs"),
       new SubmoduleDefinition("PayStubs", "Pay Stubs"),
       new SubmoduleDefinition("Deductions", "Deductions")
   }),
   ```

3. **Create domain entities** in `Payroll.Domain/Entities/`
4. **Create API controllers** in `Payroll.API/Controllers/`
5. **Create Blazor components** in `Payroll.Web/Components/`
6. **Create mobile contracts** in `Payroll.Contracts/UIModels/`
7. **Add Dockerfile** to `Payroll.API/`
8. **Update docker-compose.yml** to include payroll service

---

### When User Says: "Add a feature to existing module"

**Example:** "Add employee photo upload to HR module"

**Current Location (Monolithic):**
1. Check `backend/BusinessAsUsual.Domain/Modules/HR.Domain/` for Employee entity
2. Add `PhotoUrl` property to Employee
3. Check `backend/BusinessAsUsual.API/Controllers/` for HR controller
4. Add endpoint `POST /api/employees/{id}/photo`
5. Check `frontend/BusinessAsUsual.Web/Modules/HR/` for components
6. Update `EmployeeDetail.razor` to show photo upload

**Future Location (Microservices):**
1. Check `services/HR/HR.Domain/Entities/Employee.cs`
2. Add `PhotoUrl` property
3. Add endpoint in `services/HR/HR.API/Controllers/EmployeesController.cs`
4. Update `services/HR/HR.Web/Components/EmployeeDetail.razor`
5. Update `services/HR/HR.Contracts/UIModels/EmployeeViewModel.cs`
6. Add photo field to `EmployeeFormSpec.cs`

---

### When User Says: "Fix a bug in the UI"

**Web UI (Blazor):**
- Current: `frontend/BusinessAsUsual.Web/`
- Future: `services/{Module}/{Module}.Web/`

**Admin UI (Razor Pages):**
- Always: `frontend/BusinessAsUsual.Admin/Areas/Admin/`

**Mobile UI:**
- Don't edit mobile code (separate repo)
- Instead: Update UI contracts in `services/{Module}/{Module}.Contracts/`

---

### When User Says: "The API isn't working"

**Current Monolithic:**
1. Check `backend/BusinessAsUsual.API/Controllers/`
2. Check `backend/BusinessAsUsual.Application/` for business logic
3. Check `backend/BusinessAsUsual.Infrastructure/` for data access

**Future Microservices:**
1. Identify which service (HR, Accounting, etc.)
2. Check `services/{Service}/{Service}.API/Controllers/`
3. Check application layer for use cases
4. Check infrastructure for data access
5. Review logs in Admin portal or CloudWatch

---

### When User Says: "Deploy to production"

**Current (Monolithic):**
```bash
# Build all projects
docker-compose build

# Push to registry
docker-compose push

# Deploy to AWS EC2
ssh ec2-user@<instance>
docker-compose pull
docker-compose up -d
```

**Future (Microservices):**
```bash
# Each service deploys independently via GitHub Actions
# Push to main branch triggers:
# 1. Build service
# 2. Run tests
# 3. Build Docker image
# 4. Push to ECR
# 5. Update ECS task definition
# 6. Deploy to staging → Run smoke tests → Deploy to prod
```

---

### When User Says: "How do I test this?"

**Backend (API):**
- Unit tests: `services/{Service}/{Service}.Tests/Unit/`
- Integration tests: `services/{Service}/{Service}.Tests/Integration/`
- Run: `dotnet test services/{Service}/{Service}.Tests/`

**Frontend (Blazor):**
- Component tests: Use bUnit library
- E2E tests: Use Playwright
- Run: `dotnet test frontend/BusinessAsUsual.Web.Tests/`

**Mobile:**
- Unit tests in Android repo
- E2E tests: Use Appium or Espresso (Android)

---

### When User Says: "Show me the architecture"

**Point to:**
1. This document (`docs/HANDOVER_DOCUMENT.md`)
2. Architecture diagram (target state in this doc)
3. Module catalog (`BusinessAsUsual.Core/Modules/ModuleCatalog.cs`)
4. Docker compose file (`docker-compose.yml`)

---

### When User Says: "Where is the database schema?"

**Current:**
- Connection string in `.env` file
- EF Core migrations in `backend/BusinessAsUsual.Infrastructure/Database/Migrations/`
- Apply migrations: `dotnet ef database update`

**Future:**
- Each service has own database
- Migrations per service: `services/{Service}/{Service}.Infrastructure/Migrations/`
- Apply: `dotnet ef database update --project services/{Service}/{Service}.Infrastructure/`

---

### When User Says: "How do services communicate?"

**Synchronous (REST):**
```csharp
// HR service needs accounting data
var httpClient = _httpClientFactory.CreateClient("AccountingService");
var response = await httpClient.GetAsync("/api/cost-centers");
```

**Asynchronous (Events):**
```csharp
// HR service publishes event
await _eventBus.PublishAsync(new EmployeeHiredEvent
{
    EmployeeId = employee.Id,
    HireDate = employee.HireDate
});

// Accounting service subscribes
public class EmployeeHiredHandler : IEventHandler<EmployeeHiredEvent>
{
    public async Task Handle(EmployeeHiredEvent evt)
    {
        // Create cost center allocation
    }
}
```

---

### Common Patterns to Follow

#### 1. Clean Architecture Layers
```
API/Web (Presentation)
    ↓
Application (Use Cases)
    ↓
Domain (Business Logic)
    ↑
Infrastructure (Data Access)
```

#### 2. CQRS Pattern
```csharp
// Commands (write operations)
public record CreateEmployeeCommand(string FirstName, string LastName);
public class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand> { }

// Queries (read operations)
public record GetEmployeeQuery(string Id);
public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery> { }
```

#### 3. Blazor Component Base Classes
```csharp
// All module components inherit from this
public abstract class ModuleComponentBase : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    protected void ShowSuccess(string message) => 
        Snackbar.Add(message, Severity.Success);
}
```

#### 4. Mobile UI Spec Pattern
```csharp
public interface IUISpecification
{
    string Title { get; }
    string Icon { get; }
    bool Searchable { get; }
}

public class EmployeeListSpec : IUISpecification
{
    public string Title => "Employees";
    public List<ColumnDefinition> Columns { get; set; }
    public List<ActionDefinition> Actions { get; set; }
}
```

---

## 📚 Key Files to Remember

### Configuration Files
- **Solution:** `BusinessAsUsual.sln`
- **Docker:** `docker-compose.yml`
- **Environment:** `.env` (not in repo, create locally)
- **GitHub Actions:** `.github/workflows/`

### Module Catalog
- **Location:** `BusinessAsUsual.Core/Modules/ModuleCatalog.cs`
- **Purpose:** Central registry of all business modules
- **Update when:** Adding a new module

### Entry Points
- **API:** `backend/BusinessAsUsual.API/Program.cs`
- **Web:** `frontend/BusinessAsUsual.Web/Program.cs`
- **Admin:** `frontend/BusinessAsUsual.Admin/Program.cs`

### Shared Infrastructure
- **Monitoring:** `backend/BusinessAsUsual.Infrastructure/Monitoring/`
- **Provisioning:** `backend/BusinessAsUsual.Infrastructure/Provisioning/`
- **AWS Services:** `backend/BusinessAsUsual.Infrastructure/Extensions/`

### Documentation
- **Main README:** `README.md`
- **Architecture:** `docs/ARCHITECTURE.md`
- **This Guide:** `docs/HANDOVER_DOCUMENT.md`
- **Onboarding:** `docs/ONBOARDING.md`

---

## 🚀 Quick Start Guide

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop
- Visual Studio 2022+ (user has VS 2026 Community 18.5.2)
- SQL Server (or use Docker container)
- Git

### Local Development Setup

```bash
# 1. Clone repository
git clone https://github.com/cruckman900/BusinessAsUsual.git
cd BusinessAsUsual

# 2. Create .env file (copy from .env.example)
# Add your SQL connection string and AWS credentials

# 3. Start all services with Docker
docker-compose up --build

# 4. Or run individually in Visual Studio:
# - Set BusinessAsUsual.API as startup project (F5)
# - Set BusinessAsUsual.Web as startup project (F5)
# - Set BusinessAsUsual.Admin as startup project (F5)
```

### Access URLs
- **API:** https://localhost:5001/swagger (local) or http://localhost:5000 (Docker)
- **Web:** https://localhost:7229 (local) or http://localhost:3000 (Docker)
- **Admin:** https://localhost:7238 (local) or http://localhost:8080 (Docker)

### Run Tests
```bash
dotnet test BusinessAsUsual.Tests
```

### Common Commands
```bash
# Build solution
dotnet build

# Run specific project
dotnet run --project backend/BusinessAsUsual.API

# Apply database migrations
dotnet ef database update --project backend/BusinessAsUsual.Infrastructure

# Create new migration
dotnet ef migrations add MigrationName --project backend/BusinessAsUsual.Infrastructure
```

---

## 📋 Current Development Status

### Transition Phase: Monolith → Microservices

| Component           | Current State    | Target State      |
|---------------------|------------------|-------------------|
| **Architecture**    | Monolithic       | Microservices     |
| **API**             | Single API       | Service per module|
| **Web UI**          | Built-in modules | Injected components|
| **Mobile**          | REST only        | REST + UI contracts|
| **Deployment**      | Single container | Per-service containers|

### Module Development Status

| Module           | Domain | API | Web UI | Contracts | Status |
|------------------|--------|-----|--------|-----------|--------|
| Core/Platform    | ✅     | ✅  | ✅     | ⏳        | Stable |
| Infrastructure   | ✅     | ✅  | N/A    | N/A       | Stable |
| Identity/Auth    | ✅     | ✅  | ✅     | ⏳        | Stable |
| HR               | 📁     | ⏳  | ⏳     | ⏳        | **Next to extract** |
| Accounting       | 📁     | ⏳  | ⏳     | ⏳        | Planned |
| Inventory        | 📁     | ⏳  | ⏳     | ⏳        | Planned |
| Timekeeping      | 📁     | ⏳  | ⏳     | ⏳        | Planned |

**Legend:**
- ✅ Implemented
- ⏳ Planned
- 📁 Exists in monolith (ready to extract)

---

## 🎯 Immediate Next Actions (Priority Order)

### 1. **Extract HR Service** (Phase 1) 🔥
This is the **first microservice** to extract from the monolith.

**Deployment Target:** High-Traffic EC2 Instance (t3.small)

**Tasks:**
- [ ] Create `services/HR/` directory structure
- [ ] Move domain models from `backend/BusinessAsUsual.Domain/Modules/HR.Domain/`
- [ ] Create HR.API with controllers
- [ ] Create HR.Web with Blazor components
- [ ] Create HR.Contracts with mobile UI models
- [ ] Implement component injection in main web app
- [ ] Add to `docker-compose.hightraffic.yml`
- [ ] Deploy to high-traffic EC2
- [ ] Update ALB routing: `/api/hr/*` → high-traffic instance
- [ ] Test end-to-end flow

**Estimated Timeline:** 2-3 weeks  
**Cost Impact:** $0 (uses existing infrastructure)

### 2. **Implement Dynamic Component Loading**
Update `BusinessAsUsual.Web` to load service components at runtime.

**Tasks:**
- [ ] Create `IModuleRegistry` service
- [ ] Implement assembly scanning for `*.Web.dll`
- [ ] Register discovered Razor components
- [ ] Update MainLayout to render injected components
- [ ] Create `ModuleMetadata` attribute for route registration
- [ ] Test with HR.Web module

**Estimated Timeline:** 1 week  
**Cost Impact:** $0

### 3. **Provision Additional EC2 Instances** 💰
Set up cost-optimized infrastructure.

**Tasks:**
- [ ] Launch t3.small EC2 for high-traffic services
- [ ] Launch t3.micro EC2 for low-traffic services
- [ ] Configure security groups (same VPC)
- [ ] Install Docker & Docker Compose on each
- [ ] Create `docker-compose.hightraffic.yml`
- [ ] Create `docker-compose.lowtraffic.yml`
- [ ] Update ALB target groups
- [ ] Configure health checks per instance
- [ ] Set up CloudWatch monitoring per instance
- [ ] Test failover and routing

**Estimated Timeline:** 2-3 days  
**Cost Impact:** +$23/mo (t3.small $15 + t3.micro $8)

### 4. **Create Mobile UI Contract Pattern**
Establish pattern for mobile apps to consume UI specifications.

**Tasks:**
- [ ] Define `IUISpecification` interface
- [ ] Create `ColumnDefinition`, `ActionDefinition` models
- [ ] Implement `UISpecController` in services
- [ ] Document pattern for Android team
- [ ] Create example: `EmployeeListSpec`, `EmployeeFormSpec`

**Estimated Timeline:** 3-5 days  
**Cost Impact:** $0

### 5. **Set Up Cost Monitoring Dashboard**
Track AWS costs and resource usage in Admin Portal.

**Tasks:**
- [ ] Add AWS Cost Explorer API integration
- [ ] Create `CostDashboardViewModel.cs`
- [ ] Build cost dashboard in Admin portal
- [ ] Show per-instance utilization
- [ ] Show per-service resource usage
- [ ] Add cost alerts (Slack/Teams)
- [ ] Set up AWS Budgets ($50, $80, $100 thresholds)

**Estimated Timeline:** 3-4 days  
**Cost Impact:** $0 (using existing CloudWatch)

### 6. **Extract Additional Services (In Order)**

**High-Traffic Services → t3.small instance:**
1. Accounting Service - Port 5020
2. Inventory Service - Port 5030  
3. Timekeeping Service - Port 5040
4. Sales/CRM Service - Port 5050

**Low-Traffic Services → t3.micro instance:**
5. Assets Management - Port 5100
6. Facilities - Port 5110
7. Marketing - Port 5120
8. Support/Helpdesk - Port 5130
9. Projects - Port 5140

**For each service:**
- Follow Phase 1 pattern
- Add to appropriate docker-compose file
- Deploy to correct EC2 instance
- Update ALB routing
- Monitor resource usage (move if needed)

### 7. **Implement Service-to-Service Communication**
Add REST + Events for inter-service communication.

**Cost Consideration:** Use RabbitMQ on platform instance (already planned)  
**Cost Impact:** $0 (runs on existing t3.medium)

---

## 💡 Key Architectural Decisions

### Decision Log

| Decision | Rationale | Date | Status |
|----------|-----------|------|--------|
| **Microservices over monolith** | Better scalability, independent deployment, team autonomy | 2025-01 | In Progress |
| **Service includes UI components** | Backend controls UI, consistency across platforms, reduces mobile updates | 2025-01 | Planned |
| **Database per service** | True isolation, independent scaling, easier to reason about | 2025-01 | Planned |
| **Blazor Server over WASM** | Richer interactions, easier state management, less client overhead | 2024 | Implemented |
| **MudBlazor for components** | Material Design, comprehensive component library, active community | 2024 | Implemented |
| **Clean Architecture** | Testability, maintainability, framework independence | 2024 | Implemented |
| **CQRS pattern** | Separation of reads/writes, better performance, clearer intent | 2025-01 | Planned |
| **JWT for auth** | Stateless, works across services, mobile-friendly | 2024 | Implemented |

---

## 🤝 Integration Points Summary

### Web ↔ API
- **Protocol:** HTTP/REST
- **Auth:** JWT tokens
- **CORS:** Configured in API for web origin

### Admin ↔ API
- **Protocol:** HTTP/REST
- **Auth:** JWT tokens
- **Special:** Provisioning endpoints, monitoring

### Mobile ↔ API
- **Protocol:** HTTP/REST
- **Auth:** JWT tokens
- **Special:** UI specification endpoints (`/api/{module}/ui/{spec}`)

### Service ↔ Service (Future)
- **Sync:** HTTP/REST
- **Async:** Message bus (RabbitMQ/Service Bus)
- **Auth:** Service-to-service tokens

### Web ↔ Service Components (Future)
- **Mechanism:** Dynamic assembly loading
- **Discovery:** Scan for `*.Web.dll` in modules folder
- **Registration:** Reflection-based component registration
- **Routing:** Metadata-driven route registration

---

## 📞 Support & Resources

### Repository Information
- **Main Repo:** https://github.com/cruckman900/BusinessAsUsual
- **Android Repo:** https://github.com/cruckman900/BusinessAsUsual-Android
- **Branch:** main
- **License:** Proprietary

### User Environment
- **IDE:** Visual Studio Community 2026 (18.5.2)
- **Workspace:** `D:\DotNet Projects\BusinessAsUsual\`
- **Shell:** PowerShell
- **Target Framework:** .NET 9.0

### Documentation
- **This Guide:** `docs/HANDOVER_DOCUMENT.md`
- **Architecture:** `docs/ARCHITECTURE.md`
- **Onboarding:** `docs/ONBOARDING.md`
- **Themes:** `docs/Themes/README.md`
- **Branding:** `docs/BRANDING.md`

---

## 🎓 Learning Resources (For AI Context)

### Technologies Used
- **ASP.NET Core 9.0** - https://docs.microsoft.com/aspnet/core
- **Blazor** - https://docs.microsoft.com/aspnet/core/blazor
- **MudBlazor** - https://mudblazor.com
- **Entity Framework Core** - https://docs.microsoft.com/ef/core
- **Docker** - https://docs.docker.com
- **AWS** - https://docs.aws.amazon.com
- **Serilog** - https://serilog.net

### Patterns & Practices
- **Clean Architecture** - Robert C. Martin
- **CQRS** - Command Query Responsibility Segregation
- **Domain-Driven Design** - Eric Evans
- **Microservices Patterns** - Chris Richardson
- **Micro-Frontends** - Cam Jackson

---

**Document End**

*This AI Assistant Context Guide provides GitHub Copilot with comprehensive understanding of the BusinessAsUsual platform architecture, current state, target state, and common development patterns. Use this as reference when assisting with code generation, debugging, and architectural questions.*

---

## 🔖 Quick Reference Cheatsheet

### File Locations Cheatsheet

| Need to... | Look here... |
|------------|--------------|
| Add a domain entity | `backend/BusinessAsUsual.Domain/Modules/{Module}.Domain/` |
| Add an API endpoint | `backend/BusinessAsUsual.API/Controllers/` or `services/{Module}/{Module}.API/Controllers/` |
| Add a Blazor component | `frontend/BusinessAsUsual.Web/Modules/{Module}/` or `services/{Module}/{Module}.Web/Components/` |
| Add a Razor Page (admin) | `frontend/BusinessAsUsual.Admin/Areas/Admin/` |
| Add a mobile UI spec | `services/{Module}/{Module}.Contracts/Specifications/` |
| Add a business module | `BusinessAsUsual.Core/Modules/ModuleCatalog.cs` |
| Configure AWS services | `backend/BusinessAsUsual.Infrastructure/Extensions/` |
| Add infrastructure service | `backend/BusinessAsUsual.Infrastructure/` |
| Add tests | `BusinessAsUsual.Tests/` or `services/{Module}/{Module}.Tests/` |
| Update Docker config | `docker-compose.yml` or `{Project}/Dockerfile` |

### Port Reference

| Service | Local (HTTPS) | Local (HTTP) | Docker | Production |
|---------|---------------|--------------|--------|------------|
| API | 5001 | 5000 | 5000 | 443 (ALB) |
| Web | 7229 | - | 3000 | 443 (ALB) |
| Admin | 7238 | - | 8080 | 443 (ALB) |
| HR.API (future) | 5011 | 5010 | 5010 | 443 (ALB) |
| Acct.API (future) | 5021 | 5020 | 5020 | 443 (ALB) |

### Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| SQL Server not connecting | Check `.env` file has correct connection string |
| Docker build fails | Run `docker-compose down -v` then `docker-compose up --build` |
| Blazor circuit disconnects | Check CloudWatch middleware only runs in production |
| CORS errors | Verify frontend URL in API's `AllowAdmin` CORS policy |
| Can't find module | Check `ModuleCatalog.cs` has module registered |
| Component not rendering | Ensure component inherits from `ComponentBase` or `ModuleComponentBase` |
