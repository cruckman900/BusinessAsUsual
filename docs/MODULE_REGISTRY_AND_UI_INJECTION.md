# Module Registry Service & UI Injection

This document explains how the Business As Usual platform implements dynamic module discovery and UI injection using the Module Registry Service (MRS).

## Architecture Overview

The platform follows a **microservices + micro-frontends** architecture where:

1. **Each microservice is self-contained** with its own API, data, and UI components
2. **Module Registry Service (MRS)** acts as a central directory for all modules
3. **Web application dynamically discovers** available modules at runtime
4. **UI components are injected** into the main app shell based on module metadata

## Components

### 1. Module Registry Service (MRS)

**Location:** `services/ModuleRegistry/`

**Purpose:** Central directory that tracks all available modules in the ecosystem

**Key Features:**
- Module registration endpoint (`POST /api/modules/register`)
- Module discovery endpoints (`GET /api/modules`, `/api/modules/active`, `/api/modules/ui`)
- Health status tracking
- Module metadata storage

**Database:** SQL Server - `BusinessAsUsual_ModuleRegistry`

**Endpoints:**
- `GET /api/modules` - Get all registered modules
- `GET /api/modules/{moduleId}` - Get specific module
- `GET /api/modules/active` - Get active modules only
- `GET /api/modules/ui` - Get modules with UI components
- `POST /api/modules/register` - Register/update a module
- `GET /health` - Health check

### 2. HR Microservice (Example)

**Location:** `services/HR/`

**Self-Registration:** On startup, the HR service registers itself with MRS:

```csharp
var request = new RegisterModuleRequest
{
    ModuleId = "hr",
    DisplayName = "Human Resources",
    Description = "Manage employees, departments, onboarding, and benefits",
    Version = "1.0.0",
    ApiBaseUrl = "http://localhost:5001",
    UiEntryPoint = "http://localhost:5002/hr",
    Icon = "mdi-account-group",
    Permissions = ["hr.read", "hr.write", "hr.admin"],
    Capabilities = ["employees", "departments", "onboarding", "benefits"],
    HealthUrl = "http://localhost:5001/health",
    TenantMode = "tenant-per-database"
};
```

### 3. Web Application (Shell)

**Location:** `frontend/BusinessAsUsual.Web/`

**Module Discovery:** Uses `ModuleDiscoveryService` to fetch modules from MRS

**Dynamic Navigation:** `MainLayout` component loads modules at runtime and builds navigation dynamically

**Fallback:** If MRS is unavailable, falls back to hardcoded module list

## Data Flow

```
┌─────────────────┐
│   HR Service    │  ──────► POST /api/modules/register
└─────────────────┘              │
                                 ▼
                          ┌──────────────┐
                          │     MRS      │
                          │  (Registry)  │
                          └──────────────┘
                                 ▲
                                 │ GET /api/modules/ui
                                 │
                          ┌──────────────┐
                          │   Web App    │
                          │   (Shell)    │
                          └──────────────┘
                                 │
                                 ▼
                          Dynamically builds
                          navigation menu
```

## Running the Services

### 1. Start Module Registry Service

```bash
cd services/ModuleRegistry/ModuleRegistry.API
dotnet run
```

Access at: `http://localhost:5100`
Swagger: `http://localhost:5100/swagger`

### 2. Start HR Service

```bash
cd services/HR/HR.API
dotnet run
```

The HR service will automatically register with MRS on startup.

### 3. Start Web Application

```bash
cd frontend/BusinessAsUsual.Web
dotnet run
```

The Web app will discover modules from MRS and display them in the navigation.

## Configuration

### Module Registry Service

**Environment Variable:**
- `MRS_SQL_CONNECTION_STRING` - Database connection string

### HR Service

**appsettings.json:**
```json
{
  "ModuleRegistry": {
    "Url": "http://localhost:5100"
  },
  "HR": {
    "ApiBaseUrl": "http://localhost:5001",
    "UiEntryPoint": "http://localhost:5002"
  }
}
```

### Web Application

**appsettings.json:**
```json
{
  "ModuleRegistry": {
    "Url": "http://localhost:5100"
  }
}
```

## Adding New Modules

To add a new microservice module:

1. **Create the service structure** following the HR service example
2. **Add module registration** in `Program.cs`:
   ```csharp
   builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();
   ```
3. **Register on startup**:
   ```csharp
   var registrationService = scope.ServiceProvider.GetRequiredService<IModuleRegistrationService>();
   await registrationService.RegisterWithModuleRegistryAsync();
   ```
4. **Configure module metadata** in `appsettings.json`
5. **Start the service** - it will automatically appear in the Web app navigation

## Key Benefits

✅ **No code changes required** to add new modules to the UI  
✅ **Modules are independently deployable** with their own lifecycle  
✅ **Resilient** - fallback to hardcoded modules if MRS is down  
✅ **Tenant-aware** - each module manages its own tenant data  
✅ **Scalable** - modules can scale independently based on demand  
✅ **Clean separation** - modules don't need to know about each other  

## Future Enhancements

- [ ] Module health checking with background worker
- [ ] Module versioning and compatibility checks
- [ ] Module permissions and RBAC integration
- [ ] Module UI component lazy loading
- [ ] Module event bus for inter-module communication
- [ ] Module marketplace/catalog UI
- [ ] Module deployment automation

## References

- [Microservice Architecture Overview](../docs/MICROSERVICEARCHITECTUREOVERVIEW.md)
- [Handover Document](../docs/HANDOVER_DOCUMENT.md)
- [HR Service](services/HR/)
- [Module Registry Service](services/ModuleRegistry/)
