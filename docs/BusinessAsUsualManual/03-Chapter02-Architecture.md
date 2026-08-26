# Chapter 2: System Architecture

[CHAPTER START - Page 6]

## 2.1 Architectural Overview

Business As Usual is built using a **modern microservices architecture** where each business domain (HR, Sales, Finance, etc.) is implemented as an independent, loosely-coupled service. This approach provides flexibility, scalability, and maintainability while allowing teams to work independently.

[INSERT CHART: High-Level System Architecture Diagram]

### Core Architectural Principles

**1. Domain-Driven Design (DDD)**
- Each module represents a bounded context
- Rich domain models with business logic
- Ubiquitous language within each domain
- Anti-corruption layers between domains

**2. Microservices**
- Independent deployment and scaling
- Technology diversity where beneficial
- Failure isolation
- Polyglot persistence (where appropriate)

**3. API-First**
- Every operation exposed via REST API
- Swagger/OpenAPI documentation
- Version management
- Backward compatibility

**4. Clean Architecture**
- Domain layer (entities, business rules)
- Application layer (use cases, services)
- Infrastructure layer (data access, external services)
- Presentation layer (web UI, API controllers)

**5. CQRS (Command Query Responsibility Segregation)**
- Separate read and write models
- Optimized queries for reporting
- Event-driven updates
- Eventual consistency where appropriate

---

## 2.2 System Layers

### 2.2.1 Presentation Layer

**Technology**: Blazor (Server & WebAssembly), MudBlazor UI Components

The presentation layer provides the user interface and is architected for:

**Blazor Server**:
- Initial page loads
- SEO-friendly public pages
- Lower bandwidth requirements
- Server-side state management

**Blazor WebAssembly**:
- Rich interactive experiences
- Offline capability (PWA)
- Reduced server load
- Client-side state

**Key Components**:
- **Pages**: Routable components (`/hr/employees`, `/sales/quotes`)
- **Shared Components**: Reusable UI elements
- **Layouts**: Consistent page structure
- **Services**: Frontend state management
- **API Clients**: HTTP communication to backend

**Shared UI Library** (`BusinessAsUsual.Shared.UI`):
- `CustomDataGrid`: Reusable data table with sorting, filtering, pagination
- `CustomerPicker`: Autocomplete customer selection
- `ProductPicker`: Autocomplete product selection
- `EmployeePicker`: Employee selection with active filtering
- `DepartmentPicker`: Department selection

---

### 2.2.2 API Gateway Layer

**Technology**: ASP.NET Core, Ocelot (planned)

Currently, each module exposes its own API. Planned API Gateway will provide:

**Unified Entry Point**:
- Single hostname for all APIs
- Route aggregation
- Request/response transformation
- Protocol translation

**Cross-Cutting Concerns**:
- Authentication/Authorization
- Rate limiting
- Request logging
- Caching
- CORS handling

**Service Discovery**:
- Dynamic service registration
- Health checks
- Load balancing
- Circuit breaker patterns

---

### 2.2.3 Application Services Layer

**Technology**: ASP.NET Core Web API, MediatR

Each module has its own API project structured as:

```
ServiceName.API/
├── Controllers/          # API endpoints
├── DTOs/                # Data transfer objects
├── Middleware/          # Request pipeline
├── Filters/             # Cross-cutting concerns
└── Program.cs           # Startup configuration
```

**Key Responsibilities**:
- HTTP request handling
- Input validation
- DTO mapping
- Authorization
- API versioning
- Swagger documentation

**API Patterns**:
- RESTful conventions (GET, POST, PUT, DELETE)
- Consistent response formats
- Error handling and problem details
- HATEOAS links (planned)

---

### 2.2.4 Business Logic Layer

**Technology**: .NET 9 Class Libraries, MediatR, FluentValidation

Structured into three sub-layers:

#### Application Layer
**Purpose**: Use cases and application services

```
ServiceName.Application/
├── Commands/           # Write operations
├── Queries/           # Read operations
├── DTOs/              # Data transfer objects
├── Interfaces/        # Contracts
├── Services/          # Application services
├── Validators/        # Business rules
└── Mappings/          # Object mapping profiles
```

**Patterns**:
- **CQRS**: Commands modify state, queries read state
- **MediatR**: Decoupled request/handler pattern
- **Pipeline Behaviors**: Cross-cutting concerns (logging, validation, transactions)

Example Command:
```csharp
public class CreateEmployeeCommand : IRequest<EmployeeDto>
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Email { get; set; }
	public DateTime HireDate { get; set; }
}

public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
	// Implementation
}
```

#### Domain Layer
**Purpose**: Core business entities and rules

```
ServiceName.Domain/
├── Entities/          # Domain models
├── ValueObjects/      # Immutable values
├── Enums/            # Enumerations
├── Events/           # Domain events
└── Interfaces/       # Repository contracts
```

**Characteristics**:
- Rich domain models with behavior
- Business invariants enforced
- No infrastructure dependencies
- Framework-agnostic

Example Entity:
```csharp
public class Employee : BaseEntity
{
	public string FirstName { get; private set; }
	public string LastName { get; private set; }
	public string Email { get; private set; }
	public EmployeeStatus Status { get; private set; }

	public void Hire(DateTime hireDate)
	{
		if (Status != EmployeeStatus.Applicant)
			throw new InvalidOperationException("Only applicants can be hired");

		HireDate = hireDate;
		Status = EmployeeStatus.Active;
		AddDomainEvent(new EmployeeHiredEvent(Id));
	}
}
```

#### Contracts Layer
**Purpose**: Shared contracts between modules

```
ServiceName.Contracts/
├── Events/           # Integration events
├── DTOs/             # Shared data structures
└── Interfaces/       # Service contracts
```

---

### 2.2.5 Data Access Layer

**Technology**: Entity Framework Core 9.0, SQL Server

```
ServiceName.Infrastructure/
├── Data/
│   ├── DbContext/           # EF Core context
│   ├── Configurations/      # Entity configurations
│   ├── Migrations/          # Schema versioning
│   └── Repositories/        # Data access implementations
├── Services/                # External service integrations
└── DependencyInjection.cs   # IoC registration
```

**Database Per Service**:
Each module has its own database to ensure:
- Data encapsulation
- Independent schema evolution
- Failure isolation
- Technology flexibility

**Current Databases**:
- `BAU_Platform` - Core platform and import services
- `BAU_HR` - Human resources data
- `BAU_CRM` - Customer relationship data
- `BAU_Sales` - Sales transactions
- `BAU_Finance` - Financial records
- `BAU_Inventory` - Product and stock data
- `BAU_Services` - Project and service data
- `BAU_LMS` - Learning content

**Entity Framework Features Used**:
- Code-first migrations
- Fluent API configurations
- Query filters (soft delete, multi-tenancy)
- Shadow properties (audit fields)
- Owned entities (value objects)
- Table splitting (complex entities)

---

### 2.2.6 Cross-Cutting Concerns

**Authentication & Authorization**:
- **ASP.NET Core Identity**: User management
- **JWT Tokens**: API authentication
- **OAuth2/OpenID Connect**: Third-party integration
- **Role-Based Access Control**: Permission management
- **Claims-Based Authorization**: Fine-grained security

**Logging & Monitoring**:
- **Serilog**: Structured logging
- **Application Insights**: Telemetry and performance
- **Seq**: Log aggregation (development)
- **ELK Stack**: Production log analysis (planned)

**Caching**:
- **In-Memory**: Fast local cache
- **Redis**: Distributed cache for scale-out
- **Response Caching**: HTTP-level caching
- **Query Caching**: Database query optimization

**Error Handling**:
- Global exception middleware
- Problem details (RFC 7807)
- Custom exception types
- Retry policies (Polly)

**Validation**:
- FluentValidation rules
- Data annotations
- Domain validation
- Client-side validation (Blazor)

---

## 2.3 Module Structure

### Standard Module Layout

Each business module follows this consistent structure:

```
services/
└── ModuleName/
	├── ModuleName.API/           # REST API endpoints
	├── ModuleName.Application/   # Use cases, services
	├── ModuleName.Domain/        # Entities, business rules
	├── ModuleName.Infrastructure/# Data access, external services
	├── ModuleName.Contracts/     # Shared contracts
	├── ModuleName.Web/           # Blazor UI
	└── ModuleName.Tests/         # Unit & integration tests
```

### Module Communication

**Synchronous**: Direct HTTP calls for immediate responses
- Customer lookup from Sales module
- Employee verification from HR module
- Inventory check from Sales module

**Asynchronous**: Event-driven for eventual consistency
- Order placed → Update inventory → Notify finance
- Employee terminated → Revoke access → Archive data
- Payment received → Update AR → Notify sales

**Message Broker** (Planned):
- RabbitMQ or Azure Service Bus
- Pub/sub pattern for events
- Guaranteed delivery
- Dead letter queues

---

## 2.4 Data Flow Architecture

[INSERT CHART: Data Flow Diagram]

### Typical Request Flow

**1. User Action (Frontend)**
```
User clicks "Create Employee" button
```

**2. API Call (Blazor)**
```csharp
var response = await Http.PostAsJsonAsync("/api/employees", newEmployee);
```

**3. Controller (API Layer)**
```csharp
[HttpPost]
public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeCommand command)
{
	var result = await Mediator.Send(command);
	return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
}
```

**4. Command Handler (Application Layer)**
```csharp
public async Task<EmployeeDto> Handle(CreateEmployeeCommand request)
{
	var employee = Employee.Create(request.FirstName, request.LastName);
	await _repository.AddAsync(employee);
	await _unitOfWork.SaveChangesAsync();
	return _mapper.Map<EmployeeDto>(employee);
}
```

**5. Repository (Infrastructure Layer)**
```csharp
public async Task AddAsync(Employee employee)
{
	await _context.Employees.AddAsync(employee);
}
```

**6. Database (SQL Server)**
```sql
INSERT INTO Employees (FirstName, LastName, Email, ...)
VALUES (@p0, @p1, @p2, ...)
```

**7. Response Journey**
```
Database → Repository → Handler → Controller → API Response → Frontend → UI Update
```

---

## 2.5 Security Architecture

### Authentication Flow

[INSERT CHART: Authentication Sequence Diagram]

**1. Login Request**
- User submits credentials
- API validates against Identity database
- JWT token generated with claims

**2. Token Structure**
```json
{
  "sub": "user-guid",
  "email": "user@example.com",
  "roles": ["Employee", "Manager"],
  "tenantId": "company-guid",
  "exp": 1724700000
}
```

**3. Authorization**
- Token included in request headers
- API middleware validates signature
- Claims extracted for permission checks
- Controller actions decorated with `[Authorize]`

### Role-Based Access Control

**Predefined Roles**:
- **Super Admin**: System-wide configuration
- **Admin**: Tenant administration
- **HR Manager**: Employee management
- **HR Coordinator**: HR data entry
- **Sales Manager**: Sales oversight
- **Sales Rep**: Sales operations
- **Finance Manager**: Financial oversight
- **Accountant**: Financial operations
- **Employee**: Basic self-service
- **Guest**: Read-only access

**Permission Model**:
```csharp
[Authorize(Roles = "HR Manager,HR Coordinator")]
[Authorize(Policy = "CanViewSalaryData")]
public async Task<ActionResult> GetEmployeeSalary(Guid id)
{
	// Implementation
}
```

### Data Security

**Encryption**:
- **At Rest**: Transparent Data Encryption (TDE) on SQL Server
- **In Transit**: TLS 1.2+ for all HTTP traffic
- **Sensitive Fields**: Application-level encryption for SSN, salary

**Audit Trail**:
- All create, update, delete operations logged
- User, timestamp, old/new values captured
- Immutable audit log
- Retention policies configurable

**Row-Level Security**:
- Multi-tenant data isolation
- Users see only their tenant's data
- Global query filters in EF Core
- Fail-safe tenant validation

---

## 2.6 Scalability & Performance

### Horizontal Scaling

**Stateless Services**:
- No server-side session state
- JWT tokens for authentication
- Distributed cache for temporary data
- Load balancer friendly

**Database Scaling**:
- Read replicas for reporting
- Caching layer (Redis)
- Connection pooling
- Query optimization

**Caching Strategy**:
```
Level 1: In-Memory Cache (fast, local)
Level 2: Redis Cache (shared, distributed)
Level 3: Database (source of truth)
```

### Performance Optimizations

**Frontend**:
- Lazy loading of modules
- Virtualization for large lists
- Debouncing on search inputs
- Client-side caching
- Progressive image loading

**Backend**:
- Asynchronous operations (`async/await`)
- Bulk operations where applicable
- Projection queries (select only needed columns)
- Compiled queries
- Response compression

**Database**:
- Proper indexing strategy
- Query hints for complex operations
- Partitioning for large tables
- Archive/purge old data
- Statistics maintenance

---

## 2.7 Deployment Architecture

[INSERT CHART: Deployment Architecture Diagram]

### Container Structure

**Each Module = 2-3 Containers**:
1. **API Container**: Backend services
2. **Web Container**: Blazor UI (if separate)
3. **Worker Container**: Background jobs (if needed)

### Docker Compose (Development)

```yaml
version: '3.8'
services:
  hr-api:
	image: businessasusual/hr-api:latest
	ports:
	  - "5001:80"
	environment:
	  - ConnectionStrings__DefaultConnection=...
	  - JWT__Secret=...
	depends_on:
	  - sqlserver
	  - redis

  hr-web:
	image: businessasusual/hr-web:latest
	ports:
	  - "5101:80"
	environment:
	  - API__BaseUrl=http://hr-api

  sqlserver:
	image: mcr.microsoft.com/mssql/server:2022-latest
	environment:
	  - ACCEPT_EULA=Y
	  - SA_PASSWORD=YourStrong!Passw0rd
	volumes:
	  - sqldata:/var/opt/mssql

  redis:
	image: redis:alpine
	ports:
	  - "6379:6379"
```

### Kubernetes (Production)

**Deployment Objects**:
- **Deployments**: Application workloads
- **Services**: Internal networking
- **Ingress**: External access and routing
- **ConfigMaps**: Configuration
- **Secrets**: Sensitive data
- **PersistentVolumeClaims**: Data persistence

**High Availability**:
- Multiple replicas per service
- Pod anti-affinity rules
- Liveness and readiness probes
- Rolling updates
- Automatic failover

### Cloud Deployment Options

**Azure**:
- **Azure Kubernetes Service (AKS)**: Container orchestration
- **Azure SQL Database**: Managed database
- **Azure Cache for Redis**: Managed cache
- **Azure Front Door**: CDN and routing
- **Azure Application Insights**: Monitoring

**AWS**:
- **Elastic Kubernetes Service (EKS)**: Container orchestration
- **RDS for SQL Server**: Managed database
- **ElastiCache**: Managed Redis
- **CloudFront**: CDN
- **CloudWatch**: Monitoring

**On-Premises**:
- Docker Swarm or Kubernetes
- SQL Server (Standard/Enterprise)
- Redis cluster
- Reverse proxy (Nginx/HAProxy)
- Monitoring stack (Prometheus/Grafana)

---

## 2.8 Integration Architecture

### API Integration Points

**Each module exposes**:
- REST API with OpenAPI/Swagger documentation
- Standard CRUD operations
- Custom business operations
- Webhook registration endpoints

**Authentication**:
- OAuth2 client credentials flow
- API keys for simple integrations
- JWT tokens for user context

**Example API Call**:
```http
POST /api/v1/employees HTTP/1.1
Host: hr-api.businessasusual.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "hireDate": "2026-09-01"
}
```

### Webhook System

**Event Types**:
- `employee.created`
- `employee.terminated`
- `order.placed`
- `invoice.paid`
- `inventory.low_stock`

**Webhook Registration**:
```json
{
  "url": "https://external-system.com/webhook",
  "events": ["employee.created", "employee.terminated"],
  "secret": "webhook-signing-secret"
}
```

**Delivery Guarantees**:
- At-least-once delivery
- Retry with exponential backoff
- Signature verification (HMAC)
- Event log for replay

### Pre-Built Integrations (Planned)

**Accounting**:
- QuickBooks Online
- Xero
- Sage Intacct

**Communication**:
- Microsoft Teams
- Slack
- Twilio (SMS)
- SendGrid (Email)

**Productivity**:
- Microsoft 365
- Google Workspace
- Jira
- Trello

**Payment Processing**:
- Stripe
- PayPal
- Square
- Authorize.net

---

## 2.9 Development Environment

### Local Setup

**Prerequisites**:
- .NET 9 SDK
- Visual Studio 2026 or VS Code
- Docker Desktop
- SQL Server (local or container)
- Node.js (for frontend tooling)

**Quick Start**:
```powershell
# Clone repository
git clone https://github.com/cruckman900/BusinessAsUsual.git
cd BusinessAsUsual

# Start dependencies
docker-compose -f docker-compose.dev.yml up -d

# Run migrations
dotnet ef database update --project services/HR/HR.Infrastructure

# Start services
dotnet run --project services/HR/HR.API
dotnet run --project services/HR/HR.Web
```

### Development Tools

**IDE**:
- Visual Studio 2026 (recommended)
- Visual Studio Code + C# extension
- JetBrains Rider

**API Testing**:
- Swagger UI (built-in)
- Postman
- Insomnia
- curl / httpie

**Database**:
- SQL Server Management Studio (SSMS)
- Azure Data Studio
- DBeaver

**Monitoring**:
- Seq (local logging)
- Application Insights
- Docker Desktop dashboard

---

## 2.10 Architecture Decision Records (ADRs)

Key architectural decisions documented:

### ADR-001: Microservices Over Monolith
**Decision**: Implement as microservices from the start  
**Rationale**: Enables independent deployment, scaling, and technology choices per module  
**Trade-offs**: Increased complexity, need for service communication strategy

### ADR-002: Database Per Service
**Decision**: Each module has its own database  
**Rationale**: Data encapsulation, independent schema evolution, failure isolation  
**Trade-offs**: No cross-database joins, eventual consistency, data duplication

### ADR-003: Blazor for Frontend
**Decision**: Use Blazor (Server + WebAssembly) over Angular/React  
**Rationale**: Type safety, shared code with backend, .NET ecosystem, faster development  
**Trade-offs**: Smaller ecosystem than React, larger initial download size

### ADR-004: Entity Framework Core
**Decision**: EF Core as ORM  
**Rationale**: Strong typing, migrations, LINQ, Microsoft support  
**Trade-offs**: Performance overhead vs. raw SQL, learning curve

### ADR-005: CQRS with MediatR
**Decision**: Separate commands and queries using MediatR  
**Rationale**: Clean separation of concerns, testability, flexibility  
**Trade-offs**: More code files, learning curve for developers

### ADR-006: JWT for Authentication
**Decision**: JWT tokens over session cookies  
**Rationale**: Stateless, works across services, mobile-friendly  
**Trade-offs**: Cannot revoke tokens easily, size overhead

---

## 2.11 Summary

Business As Usual's architecture is designed for:

✅ **Scalability**: Horizontal scaling of services  
✅ **Maintainability**: Clean, layered architecture  
✅ **Flexibility**: Modular, independent services  
✅ **Performance**: Caching, async operations, optimized queries  
✅ **Security**: Multi-layered security approach  
✅ **Reliability**: Fault isolation, retry logic, monitoring  
✅ **Developer Productivity**: Modern tools, clear patterns, good DX

### Key Takeaways

- Microservices architecture with independent databases
- Clean Architecture with Domain-Driven Design
- CQRS pattern for complex operations
- Blazor for modern, fast UI
- Docker/Kubernetes for deployment
- API-first design for integration

**Next Chapter**: **Chapter 3 - Platform & Administration Module**  
Dive into the core platform features that support all other modules.

---

[CHAPTER END - Estimated 12 pages]

[Page Break]
