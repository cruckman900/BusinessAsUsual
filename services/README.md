# Business As Usual - Microservices

This directory contains all microservice modules for the Business As Usual platform. Each microservice is self-contained with its own API, data, and domain logic.

## Architecture

The platform follows a **microservices architecture** where:
- Each module is independently deployable
- Modules register themselves with the Module Registry Service (MRS)
- UI components are injected dynamically into the main web application
- Tenant-per-database isolation for data security

## Available Services

### Module Registry Service (MRS)

**Directory:** `ModuleRegistry/`  
**Port:** `5100` (HTTP), `7100` (HTTPS)  
**Purpose:** Central directory that tracks all registered modules

**Endpoints:**
- `GET /api/modules` - Get all modules
- `GET /api/modules/active` - Get active modules
- `GET /api/modules/ui` - Get modules with UI
- `POST /api/modules/register` - Register a module

**Starting:**
```bash
cd ModuleRegistry/ModuleRegistry.API
dotnet run
```

### HR Service

**Directory:** `HR/`  
**Port:** `5001` (HTTP), `7001` (HTTPS)  
**Purpose:** Human Resources management - employees, departments, onboarding

**Features:**
- Employee management
- Department management
- Self-registration with MRS

**Starting:**
```bash
cd HR/HR.API
dotnet run
```

## Service Structure

Each microservice follows Clean Architecture:

```
ServiceName/
├── ServiceName.API/              # REST API & Controllers
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
│
├── ServiceName.Application/      # Business Logic & Use Cases
│   ├── Commands/
│   ├── Queries/
│   ├── Services/
│   └── DTOs/
│
├── ServiceName.Domain/           # Domain Entities & Interfaces
│   ├── Entities/
│   ├── ValueObjects/
│   └── Repositories/
│
└── ServiceName.Infrastructure/   # Data Access & External Services
    ├── Persistence/
    ├── Repositories/
    └── Migrations/
```

## Adding a New Service

1. **Create directory structure** following the template above
2. **Implement domain logic** in `Domain` project
3. **Create application services** in `Application` project
4. **Implement data access** in `Infrastructure` project
5. **Create API controllers** in `API` project
6. **Add module registration:**

```csharp
// In Program.cs
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();

// After app.Build()
using (var scope = app.Services.CreateScope())
{
    var registrationService = scope.ServiceProvider
        .GetRequiredService<IModuleRegistrationService>();
    await registrationService.RegisterWithModuleRegistryAsync();
}
```

7. **Configure in appsettings.json:**

```json
{
  "ModuleRegistry": {
    "Url": "http://localhost:5100"
  },
  "YourService": {
    "ApiBaseUrl": "http://localhost:5xxx",
    "UiEntryPoint": "http://localhost:5002"
  }
}
```

8. **Start your service** - it will automatically register and appear in the Web UI!

## Service Communication

Services communicate via:
- **REST APIs** - For synchronous requests
- **Events** - For asynchronous operations (future)
- **Module Registry** - For service discovery

## Database Strategy

Each service uses **tenant-per-database** isolation:
- Each tenant gets its own database per service
- Naming convention: `BusinessAsUsual_{TenantId}_{ServiceName}`
- Example: `BusinessAsUsual_Acme_HR`

## Running All Services

### Option 1: Manual (Multiple Terminals)

```bash
# Terminal 1 - Module Registry
cd ModuleRegistry/ModuleRegistry.API
dotnet run

# Terminal 2 - HR Service
cd HR/HR.API
dotnet run

# Terminal 3 - Web App
cd ../../frontend/BusinessAsUsual.Web
dotnet run
```

### Option 2: Visual Studio (Recommended)

1. Right-click solution → Properties
2. Select "Multiple startup projects"
3. Set to "Start":
   - ModuleRegistry.API
   - HR.API
   - BusinessAsUsual.Web
4. Press F5

### Option 3: Docker Compose (Future)

```bash
docker-compose up
```

## Service Ports

| Service | HTTP Port | HTTPS Port |
|---------|-----------|------------|
| Module Registry | 5100 | 7100 |
| HR Service | 5001 | 7001 |
| Accounting (future) | 5003 | 7003 |
| Inventory (future) | 5004 | 7004 |
| CRM (future) | 5005 | 7005 |

## Environment Variables

Each service may use:
- `{SERVICE}_SQL_CONNECTION_STRING` - Database connection
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ModuleRegistry__Url` - MRS URL

## Testing Services

### Using HTTP Files

Each service includes a `.http` file for API testing:
- `ModuleRegistry.API/ModuleRegistry.http`
- `HR.API/HR.API.http`

Open in Visual Studio and click "Send Request"

### Using Swagger

Each service exposes Swagger UI in development:
- Module Registry: `http://localhost:5100/swagger`
- HR Service: `http://localhost:5001/swagger`

## Next Services to Add

Following the microservices roadmap:
- [ ] Accounting Service
- [ ] Inventory Service
- [ ] CRM Service
- [ ] Timekeeping Service
- [ ] Orders Service
- [ ] Projects Service

## Documentation

- [Module Registry & UI Injection Guide](../docs/MODULE_REGISTRY_AND_UI_INJECTION.md)
- [Implementation Summary](../docs/UI_INJECTION_IMPLEMENTATION_SUMMARY.md)
- [Quick Start Guide](../docs/QUICKSTART_UI_INJECTION.md)
- [Microservice Architecture Overview](../docs/MICROSERVICEARCHITECTUREOVERVIEW.md)

## Best Practices

✅ Each service owns its data  
✅ Services don't share databases  
✅ Communication via APIs and events  
✅ Self-register with Module Registry  
✅ Include health check endpoints  
✅ Follow Clean Architecture  
✅ Use DTOs for API contracts  
✅ Implement proper error handling  
✅ Add logging and monitoring  
✅ Write unit tests  

## Questions?

See the [Implementation Summary](../docs/UI_INJECTION_IMPLEMENTATION_SUMMARY.md) or check the [Handover Document](../docs/HANDOVER_DOCUMENT.md).
