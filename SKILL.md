# SKILL: Create a Complete Business Module

## Objective
Create a fully functional business module from scratch in the Business As Usual platform, including API layer, web UI, database persistence, module registration, navigation, and mobile contracts.

## Prerequisites
- .NET 9 SDK installed
- Visual Studio 2026 or later
- Understanding of the module being created (domain, features, entities)
- Module Registry API running on port 5100
- SQL Server or in-memory database configured

## Module Architecture Overview
Each module follows a clean architecture pattern:
```
services/{ModuleName}/
├── {ModuleName}.API/           # REST API endpoints
├── {ModuleName}.Web/           # Blazor UI (standalone)
├── {ModuleName}.Application/   # Business logic, DTOs, services
├── {ModuleName}.Domain/        # Domain entities, interfaces
├── {ModuleName}.Infrastructure/# Database, persistence
├── {ModuleName}.Contracts/     # Mobile UI specifications
└── {ModuleName}.Tests/         # Unit & integration tests
```

## Step-by-Step Process

### Phase 1: Module Planning & Design

#### 1.1 Choose Module Domain
**Common module suggestions:**
- **Inventory Management** - Track products, stock levels, warehouses, purchase orders
- **Project Management** - Projects, tasks, milestones, time tracking, budgets
- **Sales** - Quotes, orders, order fulfillment, shipping, sales analytics
- **Marketing** - Campaigns, leads sources, email marketing, analytics
- **Support/Helpdesk** - Tickets, knowledge base, SLA tracking, customer support
- **Asset Management** - Company assets, maintenance schedules, depreciation
- **Procurement** - Vendor management, RFQs, purchase requisitions, contracts
- **Quality Management** - Audits, inspections, non-conformances, corrective actions
- **Document Management** - Document repository, version control, approval workflows
- **Compliance** - Regulatory requirements, audits, certifications, training records

#### 1.2 Define Core Entities
List 3-7 primary entities for the module:
```
Example for Inventory:
- Product
- StockItem
- Warehouse
- PurchaseOrder
- StockAdjustment
- Supplier
- InventoryTransaction
```

#### 1.3 Define Navigation Structure
Plan the sidebar menu hierarchy:
```
Example for Inventory:
- Dashboard
- Products (group)
  - All Products
  - Categories
  - Add Product
- Stock Management (group)
  - Stock Levels
  - Adjustments
  - Transfers
- Warehouses
- Purchase Orders (group)
  - All Orders
  - Create Order
- Suppliers
- Reports
```

---

### Phase 2: Create Project Structure

#### 2.1 Create Solution Folders
```bash
cd "D:\DotNet Projects\BusinessAsUsual\services"
mkdir {ModuleName}
cd {ModuleName}

# Create projects
dotnet new webapi -n {ModuleName}.API
dotnet new blazor -n {ModuleName}.Web
dotnet new classlib -n {ModuleName}.Application
dotnet new classlib -n {ModuleName}.Domain
dotnet new classlib -n {ModuleName}.Infrastructure
dotnet new classlib -n {ModuleName}.Contracts
dotnet new xunit -n {ModuleName}.Tests
```

#### 2.2 Add Projects to Solution
**CRITICAL:** Add all projects to the solution immediately after creating them:

```bash
cd "D:\DotNet Projects\BusinessAsUsual"

dotnet sln add services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Application/{ModuleName}.Application.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Domain/{ModuleName}.Domain.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Contracts/{ModuleName}.Contracts.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Tests/{ModuleName}.Tests.csproj
```

⚠️ **Common Issue:** If you forget this step, projects won't appear in Visual Studio's startup project list!

#### 2.3 Verify .NET Target Framework
**CRITICAL:** Ensure all projects target `net9.0`, not `net10.0` or other versions.

Check each `.csproj` file:
```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  ...
</PropertyGroup>
```

If any project has `net10.0` or a different version, manually edit the `.csproj` file to fix it.

#### 2.4 Add Project References
```bash
# API references
cd services/{ModuleName}/{ModuleName}.API
```bash
# API references
cd services/{ModuleName}/{ModuleName}.API
dotnet add reference ../{ModuleName}.Application/{ModuleName}.Application.csproj
dotnet add reference ../{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj
dotnet add reference ../{ModuleName}.Contracts/{ModuleName}.Contracts.csproj

# Web references  
cd ../{ModuleName}.Web
dotnet add reference ../{ModuleName}.Application/{ModuleName}.Application.csproj
dotnet add reference ../{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj

# Application references
cd ../{ModuleName}.Application
dotnet add reference ../{ModuleName}.Domain/{ModuleName}.Domain.csproj

# Infrastructure references
cd ../{ModuleName}.Infrastructure
dotnet add reference ../{ModuleName}.Domain/{ModuleName}.Domain.csproj
```

#### 2.5 Install Required NuGet Packages
**CRITICAL:** Verify package versions match .NET 9 compatibility:

```bash
# API - USE VERSION 9.0.0 for EF Core and OpenAPI
cd services/{ModuleName}/{ModuleName}.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.0
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 9.0.0

# Infrastructure
cd ../{ModuleName}.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0

# Web - Match shell's MudBlazor version (9.6.0)
cd ../{ModuleName}.Web
dotnet add package MudBlazor --version 9.6.0

# Application
cd ../{ModuleName}.Application
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 9.0.0
```

⚠️ **Common Issue:** Using version 10.x packages will cause build failures! Always use 9.0.x for .NET 9.

---

### Phase 3: Domain Layer

#### 3.1 Create Domain Entities
Create entity classes in `{ModuleName}.Domain/Entities/`:

```csharp
// Example: Product.cs
namespace {ModuleName}.Domain.Entities;

public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string SKU { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int QuantityOnHand { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsActive { get; set; } = true;
}
```

#### 3.2 Create Repository Interfaces
Create in `{ModuleName}.Domain/Interfaces/`:

```csharp
namespace {ModuleName}.Domain.Interfaces;

public interface IProductRepository
{
	Task<IEnumerable<Product>> GetAllAsync();
	Task<Product?> GetByIdAsync(Guid id);
	Task<Product> AddAsync(Product entity);
	Task<Product> UpdateAsync(Product entity);
	Task DeleteAsync(Guid id);
}
```

---

### Phase 4: Infrastructure Layer

#### 4.1 Create DbContext
Create `{ModuleName}DbContext.cs` in `{ModuleName}.Infrastructure/Persistence/`:

```csharp
using Microsoft.EntityFrameworkCore;
using {ModuleName}.Domain.Entities;

namespace {ModuleName}.Infrastructure.Persistence;

public class {ModuleName}DbContext : DbContext
{
	public {ModuleName}DbContext(DbContextOptions<{ModuleName}DbContext> options)
		: base(options)
	{
	}

	public DbSet<Product> Products => Set<Product>();
	// Add other DbSets here

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
			entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
			entity.HasIndex(e => e.SKU).IsUnique();
		});
	}
}
```

#### 4.2 Create Repository Implementations
Create in `{ModuleName}.Infrastructure/Repositories/`:

```csharp
using {ModuleName}.Domain.Entities;
using {ModuleName}.Domain.Interfaces;
using {ModuleName}.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace {ModuleName}.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
	private readonly {ModuleName}DbContext _context;

	public ProductRepository({ModuleName}DbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Product>> GetAllAsync()
		=> await _context.Products.Where(p => p.IsActive).ToListAsync();

	public async Task<Product?> GetByIdAsync(Guid id)
		=> await _context.Products.FindAsync(id);

	public async Task<Product> AddAsync(Product entity)
	{
		entity.Id = Guid.NewGuid();
		entity.CreatedAt = DateTime.UtcNow;
		_context.Products.Add(entity);
		await _context.SaveChangesAsync();
		return entity;
	}

	public async Task<Product> UpdateAsync(Product entity)
	{
		entity.UpdatedAt = DateTime.UtcNow;
		_context.Products.Update(entity);
		await _context.SaveChangesAsync();
		return entity;
	}

	public async Task DeleteAsync(Guid id)
	{
		var entity = await _context.Products.FindAsync(id);
		if (entity != null)
		{
			entity.IsActive = false;
			await _context.SaveChangesAsync();
		}
	}
}
```

#### 4.3 Create Initial Migration
```bash
cd {ModuleName}.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../{ModuleName}.API
```

---

### Phase 5: Application Layer

#### 5.1 Create DTOs
Create in `{ModuleName}.Application/DTOs/`:

```csharp
namespace {ModuleName}.Application.DTOs;

public class ProductDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string SKU { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int QuantityOnHand { get; set; }
}

public class CreateProductRequest
{
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string SKU { get; set; } = string.Empty;
	public decimal Price { get; set; }
}
```

#### 5.2 Create Services
Create in `{ModuleName}.Application/Services/`:

```csharp
using {ModuleName}.Application.DTOs;
using {ModuleName}.Domain.Entities;
using {ModuleName}.Domain.Interfaces;

namespace {ModuleName}.Application.Services;

public interface IProductService
{
	Task<IEnumerable<ProductDto>> GetAllAsync();
	Task<ProductDto?> GetByIdAsync(Guid id);
	Task<ProductDto> CreateAsync(CreateProductRequest request);
	Task<ProductDto> UpdateAsync(Guid id, CreateProductRequest request);
	Task DeleteAsync(Guid id);
}

public class ProductService : IProductService
{
	private readonly IProductRepository _repository;

	public ProductService(IProductRepository repository)
	{
		_repository = repository;
	}

	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		var entities = await _repository.GetAllAsync();
		return entities.Select(MapToDto);
	}

	public async Task<ProductDto?> GetByIdAsync(Guid id)
	{
		var entity = await _repository.GetByIdAsync(id);
		return entity == null ? null : MapToDto(entity);
	}

	public async Task<ProductDto> CreateAsync(CreateProductRequest request)
	{
		var entity = new Product
		{
			Name = request.Name,
			Description = request.Description,
			SKU = request.SKU,
			Price = request.Price
		};

		var created = await _repository.AddAsync(entity);
		return MapToDto(created);
	}

	private static ProductDto MapToDto(Product entity)
	{
		return new ProductDto
		{
			Id = entity.Id,
			Name = entity.Name,
			Description = entity.Description,
			SKU = entity.SKU,
			Price = entity.Price,
			QuantityOnHand = entity.QuantityOnHand
		};
	}

	// Implement other methods...
}
```

#### 5.3 Create Module Registration Service
Create in `{ModuleName}.Application/Services/ModuleRegistrationService.cs`:

```csharp
using {ModuleName}.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace {ModuleName}.Application.Services;

public interface IModuleRegistrationService
{
	Task RegisterWithModuleRegistryAsync();
}

public class ModuleRegistrationService : IModuleRegistrationService
{
	private readonly HttpClient _httpClient;
	private readonly IConfiguration _configuration;

	public ModuleRegistrationService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_configuration = configuration;
	}

	public async Task RegisterWithModuleRegistryAsync()
	{
		var registryUrl = _configuration["ModuleRegistry:Url"] ?? "http://localhost:5100";
		var apiUrl = _configuration["{ModuleName}:ApiBaseUrl"] ?? "http://localhost:50XX";
		var webUrl = _configuration["{ModuleName}:UiEntryPoint"] ?? "http://localhost:50XX";

		var request = new RegisterModuleRequest
		{
			ModuleId = "{modulename}",
			Key = "{modulename}",
			DisplayName = "{ModuleName}",
			Description = "Module description here",
			Version = "1.0.0",
			ApiBaseUrl = apiUrl,
			UiEntryPoint = $"{webUrl}/{modulename}",
			Icon = Icons.Dashboard, // Choose appropriate icon
			Permissions = new List<string> { "{modulename}.read", "{modulename}.write", "{modulename}.admin" },
			Capabilities = new List<string> { "feature1", "feature2" },
			HealthUrl = $"{apiUrl}/health",
			TenantMode = "tenant-per-database",
			SupportsMobile = true,
			MobileUISpecUrl = $"{apiUrl}/api/{modulename}/mobile/ui-spec",
			MobileContractVersion = "1.0.0",
			NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
			{
				new() { Label = "Dashboard", Route = "/{modulename}", Icon = Icons.Dashboard },
				// Add navigation structure here
			}
		};

		try
		{
			var response = await _httpClient.PostAsJsonAsync($"{registryUrl}/api/modules/register", request);
			response.EnsureSuccessStatusCode();
			Console.WriteLine($"✓ Successfully registered {ModuleName} module with Module Registry");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to register with Module Registry: {ex.Message}");
		}
	}

	private static class Icons
	{
		public const string Dashboard = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z\"/>";
		// Add more icon SVG paths as needed
	}
}
```

---

### Phase 6: API Layer

#### 6.1 Create API Controllers
Create in `{ModuleName}.API/Controllers/`:

```csharp
using Microsoft.AspNetCore.Mvc;
using {ModuleName}.Application.DTOs;
using {ModuleName}.Application.Services;

namespace {ModuleName}.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly IProductService _service;

	public ProductsController(IProductService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
	{
		var result = await _service.GetAllAsync();
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<ProductDto>> GetById(Guid id)
	{
		var result = await _service.GetByIdAsync(id);
		if (result == null) return NotFound();
		return Ok(result);
	}

	[HttpPost]
	public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request)
	{
		var result = await _service.CreateAsync(request);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}
}
```

#### 6.2 Create Mobile UI Controller
Create in `{ModuleName}.API/Controllers/MobileUIController.cs`:

```csharp
using {ModuleName}.Contracts.Navigation;
using {ModuleName}.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace {ModuleName}.API.Controllers;

[ApiController]
[Route("api/{modulename}/mobile")]
public class MobileUIController : ControllerBase
{
	[HttpGet("ui-spec")]
	public ActionResult<MobileUISpecification> GetUISpecification()
	{
		var spec = new MobileUISpecification
		{
			ModuleId = "{modulename}",
			ModuleName = "{ModuleName}",
			DisplayName = "{ModuleName}",
			Version = "1.0.0",
			Navigation = GetNavigationMap(),
			Screens = new Dictionary<string, object>
			{
				// Define mobile screens here
			}
		};

		return Ok(spec);
	}

	[HttpGet("navigation")]
	public ActionResult<ModuleNavigationMap> GetNavigation() => Ok(GetNavigationMap());

	private static ModuleNavigationMap GetNavigationMap() => new()
	{
		ModuleId = "{modulename}",
		ModuleName = "{ModuleName}",
		Icon = "dashboard",
		Items = new List<NavigationItem>
		{
			new() { Id = "dashboard", Label = "Dashboard", Icon = "dashboard", Screen = "dashboard", Route = "/{modulename}" },
			// Add mobile navigation items here
		}
	};
}
```

#### 6.3 Configure Program.cs
Update `{ModuleName}.API/Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using {ModuleName}.Application.Services;
using {ModuleName}.Domain.Interfaces;
using {ModuleName}.Infrastructure.Persistence;
using {ModuleName}.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure database
var connectionString = builder.Configuration.GetConnectionString("{ModuleName}Db")
	?? "Server=localhost;Database=BusinessAsUsual_{ModuleName};Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
	options.UseSqlServer(connectionString));

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Add other repositories...

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
// Add other services...

// Register module registration service
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();
builder.Services.AddHostedService<ModuleRegistrationHostedService>();

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<{ModuleName}DbContext>();
	db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### 6.3.1 🔥 CRITICAL: Configure In-Memory Database & Seed Test Data

**Problem:** If the API uses SQL Server without proper configuration, it will fail at runtime when the database connection isn't available. The dashboard will appear blank or show only a loading spinner.

**Solution:** Add in-memory database support for development and seed it with test data.

**Step 1:** Add the in-memory package to the API:
```bash
cd services/{ModuleName}/{ModuleName}.API
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 9.0.0
```

**Step 2:** Update `Program.cs` to support in-memory database:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use in-memory database for development
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
    Console.WriteLine("⚠️  {ModuleName}.API using in-memory database");
    builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
        options.UseInMemoryDatabase("{ModuleName}_API"));
}
else
{
    builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("{ModuleName}Connection")));
}

// ... rest of DI registrations ...

var app = builder.Build();

// Seed in-memory database with test data
if (useInMemory)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<{ModuleName}DbContext>();
        SeedData(context);
    }
}

// ... rest of app configuration ...

app.Run();

static void SeedData({ModuleName}DbContext context)
{
    if (context.{PrimaryEntity}.Any()) return; // Already seeded

    // Create test entities with Guid IDs
    // ⚠️ IMPORTANT: Use Guid.NewGuid() for all IDs, not integers!
    // ⚠️ IMPORTANT: Use the exact property names from your domain entities!

    var entity1 = new {ModuleName}.Domain.Entities.{Entity}
    {
        Id = Guid.NewGuid(),
        Name = "Test Item 1",
        // ... match your entity's properties exactly ...
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    var entity2 = new {ModuleName}.Domain.Entities.{Entity}
    {
        Id = Guid.NewGuid(),
        Name = "Test Item 2",
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    context.{Entities}.AddRange(entity1, entity2);
    context.SaveChanges();

    Console.WriteLine("✅ {ModuleName} database seeded with test data");
}
```

**Why This Matters:**
- Without seed data, dashboard queries return empty results, making the UI appear broken
- The dashboard shows "Loading..." indefinitely if the API isn't running
- In-memory database lets you develop/test without SQL Server configuration
- Console messages help debug whether data was actually seeded

**Validation:**
When you run the API, you should see:
```
⚠️  {ModuleName}.API using in-memory database
✅ {ModuleName} database seeded with test data
```

#### 6.4 Create ModuleRegistrationHostedService
Create in `{ModuleName}.API/Services/`:

```csharp
using {ModuleName}.Application.Services;

namespace {ModuleName}.API.Services;

public class ModuleRegistrationHostedService : IHostedService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<ModuleRegistrationHostedService> _logger;

	public ModuleRegistrationHostedService(
		IServiceProvider serviceProvider,
		ILogger<ModuleRegistrationHostedService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var registrationService = scope.ServiceProvider.GetRequiredService<IModuleRegistrationService>();

		try
		{
			await registrationService.RegisterWithModuleRegistryAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to register module with Module Registry");
		}
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

---

### Phase 7: Mobile Contracts

#### 7.1 Create Navigation Contracts
Create in `{ModuleName}.Contracts/Navigation/ModuleNavigationMap.cs`:

```csharp
namespace {ModuleName}.Contracts.Navigation;

public class ModuleNavigationMap
{
	public string ModuleId { get; set; } = string.Empty;
	public string ModuleName { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
	public List<NavigationItem> Items { get; set; } = new();
}

public class NavigationItem
{
	public string Id { get; set; } = string.Empty;
	public string Label { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
	public string Screen { get; set; } = string.Empty;
	public string? Route { get; set; }
	public List<NavigationItem>? Children { get; set; }
	public bool RequiresPermission { get; set; } = false;
	public string? Permission { get; set; }
}
```

#### 7.2 Create Mobile UI Specification
Create in `{ModuleName}.Contracts/Specifications/MobileUISpecification.cs`:

```csharp
using {ModuleName}.Contracts.Navigation;

namespace {ModuleName}.Contracts.Specifications;

public class MobileUISpecification
{
	public string ModuleId { get; set; } = string.Empty;
	public string ModuleName { get; set; } = string.Empty;
	public string DisplayName { get; set; } = string.Empty;
	public string Version { get; set; } = string.Empty;
	public ModuleNavigationMap Navigation { get; set; } = new();
	public Dictionary<string, object> Screens { get; set; } = new();
}
```

---

### Phase 8: Web UI (Blazor)

#### 8.1 Create _Imports.razor (IMPORTANT!)
Create `{ModuleName}.Web/Components/_Imports.razor` to avoid repetitive using statements:

```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MudBlazor
@using {ModuleName}.Web.Components
@using {ModuleName}.Application.DTOs
@using {ModuleName}.Application.Services
```

**Why:** This prevents `@using MudBlazor` errors and reduces boilerplate in every page.

#### 8.2 Standard Page Layout & Breadcrumbs

**CRITICAL:** All module pages must follow this consistent layout pattern:

**Page Structure:**
1. **Container:** `MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0"`
2. **Breadcrumbs:** Manual breadcrumb trail (Dashboard → Module → Page)
3. **Page Header:** H4 with icon and optional subtitle
4. **Action Button:** (optional) Top-right aligned button for primary action
5. **Page Content:** Tables, cards, forms, etc.

**Template for Dashboard Page:**
```razor
@page "/{modulename}"
@using System.Net.Http.Json
@using MudBlazor
@inject IHttpClientFactory HttpClientFactory
@inject NavigationManager Navigation

<PageTitle>{ModuleName} Dashboard</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- BREADCRUMBS -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">{ModuleName}</MudText>
    </div>

    <!-- PAGE HEADER -->
    <MudText Typo="Typo.h4" GutterBottom="true">
        <MudIcon Icon="@Icons.Material.Filled.{ModuleIcon}" Class="mr-2" />
        {ModuleName} Dashboard
    </MudText>
    <MudText Typo="Typo.body1" Color="Color.Secondary" Class="mb-4">
        {Module description}
    </MudText>

    <!-- PAGE CONTENT -->
    <!-- ... dashboard sections ... -->
</MudContainer>
```

**Template for Sub-Pages (e.g., Products, Employees):**
```razor
@page "/{modulename}/subpage"
@using MudBlazor
@inject NavigationManager Navigation

<PageTitle>{SubPage Name}</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- BREADCRUMBS -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudLink Href="/{modulename}">{ModuleName}</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">{SubPage Name}</MudText>
    </div>

    <!-- PAGE HEADER WITH ACTION BUTTON -->
    <div class="d-flex justify-space-between align-center mb-4">
        <div>
            <MudText Typo="Typo.h4" GutterBottom="true">
                <MudIcon Icon="@Icons.Material.Filled.{Icon}" Class="mr-2" />
                {SubPage Name}
            </MudText>
            <MudText Typo="Typo.body1" Color="Color.Secondary">
                {Page description}
            </MudText>
        </div>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add">
            {Primary Action}
        </MudButton>
    </div>

    <!-- PAGE CONTENT -->
    <!-- ... tables, cards, etc. ... -->
</MudContainer>
```

**Breadcrumb Rules:**
- Always start with `/dashboard`
- Use `MudLink` for clickable breadcrumbs
- Use `<span class="mx-2">/</span>` for separators
- Final breadcrumb uses `MudText` with `Color.Primary` (not a link)
- Keep breadcrumbs hierarchical: Dashboard → Module → SubPage → Detail

**Container Rules:**
- **Always** use `MaxWidth.ExtraExtraLarge` (not `ExtraLarge`)
- **Always** use `Class="mt-2 pa-0"` (not `mt-4`)
- This ensures consistent spacing and padding across all modules

#### 8.3 Create Dashboard Page
Create in `{ModuleName}.Web/Components/Pages/Dashboard.razor`:

**CRITICAL - API Client Usage:**
- **ALWAYS** use `@inject IHttpClientFactory HttpClientFactory` (NOT `@inject HttpClient`)
- **ALWAYS** use the named client: `HttpClientFactory.CreateClient("{ModuleName}Api")`
- **NEVER** hardcode API URLs in pages - the base URL is configured in shell `Program.cs`
- All API calls should use **relative paths** (e.g., `"api/inventory/products"`, NOT `"https://localhost:7079/api/inventory/products"`)

**IMPORTANT:** A complete dashboard should include:
1. **Stats/Metrics Row** - Key numbers at the top (total items, value, alerts, etc.)
2. **Navigation Cards Section** - Clickable cards linking to major module features
3. **Quick Actions Card** - Common tasks users perform
4. **Alerts & Notifications Card** - Warnings, status messages
5. **About Module Section** - Description of module capabilities (2/3 width)
6. **Module Info Card** - Technical details: module ID, version, ports, status (1/3 width)

```razor
@page "/{modulename}"
@using System.Net.Http.Json
@using MudBlazor
@inject IHttpClientFactory HttpClientFactory
@inject NavigationManager Navigation

<PageTitle>{ModuleName} Dashboard</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-4">
	<MudText Typo="Typo.h4" GutterBottom="true">{ModuleName} Dashboard</MudText>

	@if (_loading)
	{
		<MudProgressCircular Indeterminate="true" />
		<MudText Class="mt-2">Loading dashboard data...</MudText>
	}
	else if (!string.IsNullOrEmpty(_errorMessage))
	{
		<MudAlert Severity="Severity.Error" Variant="Variant.Filled">
			<MudText>@_errorMessage</MudText>
			<MudText Class="mt-2">Please ensure {ModuleName}.API is running and accessible.</MudText>
		</MudAlert>
	}
	else if (_summary != null)
	{
		<!-- 1. STATS/METRICS ROW -->
		<MudGrid>
			<MudItem xs="12" sm="6" md="3">
				<MudCard Elevation="2">
					<MudCardContent>
						<div class="d-flex justify-space-between">
							<div>
								<MudText Typo="Typo.body2" Color="Color.Secondary">Total Products</MudText>
								<MudText Typo="Typo.h5">@_summary.TotalProducts</MudText>
							</div>
							<MudIcon Icon="@Icons.Material.Filled.Inventory" Size="Size.Large" Color="Color.Primary" />
						</div>
					</MudCardContent>
				</MudCard>
			</MudItem>
			<!-- Add 3-5 more metric cards here -->
		</MudGrid>

		<!-- 2. MODULE NAVIGATION CARDS -->
		<MudGrid Class="mt-6">
			<MudItem xs="12">
				<MudText Typo="Typo.h5" Class="mb-4">{ModuleName} Management</MudText>
			</MudItem>

			<MudItem xs="12" sm="6" md="4">
				<MudCard Elevation="2" Class="pa-4 mud-card-hover" Style="cursor: pointer;" onclick="@(() => NavigateTo("/{modulename}/products"))">
					<MudCardContent>
						<div class="d-flex align-center mb-2">
							<MudIcon Icon="@Icons.Material.Filled.Inventory" Size="Size.Large" Color="Color.Primary" Class="mr-3" />
							<MudText Typo="Typo.h6">Products</MudText>
						</div>
						<MudText Typo="Typo.body2" Color="Color.Secondary">
							Manage product catalog and pricing
						</MudText>
						<MudButton Variant="Variant.Text" Color="Color.Primary" Class="mt-2" Href="/{modulename}/products">
							View Products
						</MudButton>
					</MudCardContent>
				</MudCard>
			</MudItem>
			<!-- Add 5-7 more navigation cards for other major features -->
		</MudGrid>

		<!-- 3. QUICK ACTIONS & ALERTS -->
		<MudGrid Class="mt-6">
			<MudItem xs="12" md="6">
				<MudCard Elevation="2">
					<MudCardHeader>
						<CardHeaderContent>
							<MudText Typo="Typo.h6">Quick Actions</MudText>
						</CardHeaderContent>
					</MudCardHeader>
					<MudCardContent>
						<MudStack Spacing="2">
							<MudButton Variant="Variant.Text" StartIcon="@Icons.Material.Filled.Add" Href="/{modulename}/products/new" FullWidth="true" Class="justify-start">
								Add New Product
							</MudButton>
							<!-- Add 3-5 more common actions -->
						</MudStack>
					</MudCardContent>
				</MudCard>
			</MudItem>

			<MudItem xs="12" md="6">
				<MudCard Elevation="2">
					<MudCardHeader>
						<CardHeaderContent>
							<MudText Typo="Typo.h6">Alerts & Notifications</MudText>
						</CardHeaderContent>
					</MudCardHeader>
					<MudCardContent>
						@if (_summary.LowStockCount > 0)
						{
							<MudAlert Severity="Severity.Warning" Variant="Variant.Filled" Class="mb-2">
								<MudText>@_summary.LowStockCount items need attention</MudText>
							</MudAlert>
						}
						<!-- Add more conditional alerts -->
					</MudCardContent>
				</MudCard>
			</MudItem>
		</MudGrid>

		<!-- 4. ABOUT MODULE & MODULE INFO -->
		<MudGrid Class="mt-6">
			<MudItem xs="12" md="8">
				<MudPaper Class="pa-4 d-flex flex-column" Elevation="1" Style="height: 100%;">
					<MudText Typo="Typo.h6" GutterBottom="true">
						<MudIcon Icon="@Icons.Material.Filled.Info" Class="mr-2" />
						About {ModuleName} Module
					</MudText>
					<MudText Typo="Typo.body2" Class="mb-4">
						The {ModuleName} module provides comprehensive capabilities including:
					</MudText>
					<MudList T="string" Dense="true">
						<MudListItem T="string" Icon="@Icons.Material.Filled.Check">
							<strong>Feature 1</strong> - Description
						</MudListItem>
						<MudListItem T="string" Icon="@Icons.Material.Filled.Check">
							<strong>Feature 2</strong> - Description
						</MudListItem>
						<!-- Add 4-6 key features -->
					</MudList>
					<MudText Typo="Typo.caption" Color="Color.Secondary" Class="mt-auto pt-4">
						This module is dynamically loaded via the Module Registry Service and provides both web UI and mobile API contracts.
					</MudText>
				</MudPaper>
			</MudItem>

			<MudItem xs="12" md="4">
				<MudPaper Class="pa-4 d-flex flex-column" Elevation="1" Style="height: 100%;">
					<MudText Typo="Typo.h6" GutterBottom="true">
						<MudIcon Icon="@Icons.Material.Filled.Settings" Class="mr-2" />
						Module Info
					</MudText>
					<MudStack Spacing="2" Class="flex-grow-1">
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Module ID</MudText>
							<MudChip T="string" Size="Size.Small" Color="Color.Default" Variant="Variant.Filled">{modulename}</MudChip>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Version</MudText>
							<MudText Typo="Typo.body2">1.0.0</MudText>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">API Port</MudText>
							<MudText Typo="Typo.body2">50XX</MudText>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Web UI Port</MudText>
							<MudText Typo="Typo.body2">50YY</MudText>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Mobile Support</MudText>
							<MudChip T="string" Size="Size.Small" Color="Color.Success" Variant="Variant.Filled">Yes</MudChip>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Status</MudText>
							<MudChip T="string" Size="Size.Small" Color="Color.Success" Variant="Variant.Filled">Active</MudChip>
						</div>
					</MudStack>
				</MudPaper>
			</MudItem>
		</MudGrid>
	}
</MudContainer>

@code {
	private bool _loading = true;
	private {ModuleName}Summary? _summary;
	private string? _errorMessage;

	protected override async Task OnInitializedAsync()
	{
		try
		{
			var httpClient = HttpClientFactory.CreateClient("{ModuleName}Api");
			_summary = await httpClient.GetFromJsonAsync<{ModuleName}Summary>("/api/{modulename}/dashboard/summary");
		}
		catch (HttpRequestException ex)
		{
			_errorMessage = $"Failed to connect to {ModuleName} API: {ex.Message}";
			Console.WriteLine($"Error loading dashboard: {ex.Message}");
		}
		catch (Exception ex)
		{
			_errorMessage = $"Error loading dashboard: {ex.Message}";
			Console.WriteLine($"Error loading dashboard: {ex.Message}");
		}
		finally
		{
			_loading = false;
		}
	}

	private void NavigateTo(string url)
	{
		Navigation.NavigateTo(url);
	}

	public class {ModuleName}Summary
	{
		public int TotalProducts { get; set; }
		// Add other summary properties
	}
}

<style>
	.mud-card-hover {
		transition: transform 0.2s ease, box-shadow 0.2s ease;
	}

	.mud-card-hover:hover {
		transform: translateY(-4px);
		box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15) !important;
	}
</style>
```

---

#### 8.3.1 Create Sub-Pages with Data Loading

All sub-pages (Products, Employees, etc.) must follow these patterns:

**Using CustomDataGrid Instead of Regular Tables**

The platform provides a reusable `CustomDataGrid` component that wraps MudBlazor's `MudDataGrid` with enhanced features including built-in toolbar, search, filtering, and consistent styling. **Always use CustomDataGrid instead of creating raw MudDataGrid or HTML tables** for listing data.

**Location:** `{ModuleName}.Web/Components/Shared/CustomDataGrid.razor`

**Key Benefits:**
- Built-in search with customizable quick filter
- Automatic toolbar with title and action buttons
- Consistent styling across all modules
- Support for custom toolbar content (filters, dropdowns, etc.)
- All standard MudDataGrid features (sorting, filtering, pagination)

**Complete Example:**

```razor
@page "/{modulename}/products"
@using {ModuleName}.Web.Components.Shared
@using {ModuleName}.Application.DTOs
@inject IProductService ProductService
@inject NavigationManager Navigation
@inject ISnackbar Snackbar

<PageTitle>Products</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- Breadcrumb -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudLink Href="/{modulename}">@ModuleName</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">Products</MudText>
    </div>

    <!-- Header -->
    <MudStack Row="true" AlignItems="AlignItems.Center" Class="mb-4">
        <MudText Typo="Typo.h4" Class="flex-grow-1">
            <MudIcon Icon="@Icons.Material.Filled.Inventory" Class="mr-2" />
            Products
        </MudText>
        <MudButton Variant="Variant.Filled" 
                   Color="Color.Primary" 
                   StartIcon="@Icons.Material.Filled.Add"
                   OnClick="@(() => Navigation.NavigateTo("/{modulename}/products/new"))">
            Add Product
        </MudButton>
    </MudStack>

    @if (_loading)
    {
        <MudProgressCircular Indeterminate="true" />
    }
    else
    {
        <!-- CustomDataGrid with all features -->
        <CustomDataGrid TItem="ProductDto"
                        Items="@_products"
                        Title="Product List"
                        SearchPlaceholder="Search products..."
                        QuickFilterFunc="@((ProductDto p) => 
                            string.IsNullOrEmpty(_searchString) || 
                            (p.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (p.SKU?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false))"
                        Elevation="2">
            <ToolbarContent>
                <!-- Add custom filters or actions in the toolbar -->
                <MudSelect T="string" @bind-Value="_categoryFilter" 
                          Label="Category" 
                          Variant="Variant.Outlined" 
                          Class="ml-4" 
                          Style="min-width: 150px;">
                    <MudSelectItem Value="@("All")">All Categories</MudSelectItem>
                    <MudSelectItem Value="@("Electronics")">Electronics</MudSelectItem>
                    <MudSelectItem Value="@("Furniture")">Furniture</MudSelectItem>
                </MudSelect>
            </ToolbarContent>
            <ChildContent>
                <!-- Define columns using PropertyColumn or TemplateColumn -->
                <PropertyColumn T="ProductDto" TProperty="string" Property="p => p.Name" Title="Product Name">
                    <CellTemplate>
                        <MudText Typo="Typo.body2" Style="font-weight: 500;">@context.Item.Name</MudText>
                    </CellTemplate>
                </PropertyColumn>

                <PropertyColumn T="ProductDto" TProperty="string" Property="p => p.SKU" Title="SKU" />

                <PropertyColumn T="ProductDto" TProperty="decimal" Property="p => p.Price" Title="Price">
                    <CellTemplate>
                        @context.Item.Price.ToString("C2")
                    </CellTemplate>
                </PropertyColumn>

                <PropertyColumn T="ProductDto" TProperty="int" Property="p => p.QuantityOnHand" Title="Stock">
                    <CellTemplate>
                        <MudChip T="string" 
                                Size="Size.Small" 
                                Color="@(context.Item.QuantityOnHand > 10 ? Color.Success : Color.Warning)">
                            @context.Item.QuantityOnHand
                        </MudChip>
                    </CellTemplate>
                </PropertyColumn>

                <PropertyColumn T="ProductDto" TProperty="bool" Property="p => p.IsActive" Title="Status">
                    <CellTemplate>
                        <MudChip T="string" 
                                Size="Size.Small" 
                                Color="@(context.Item.IsActive ? Color.Success : Color.Default)">
                            @(context.Item.IsActive ? "Active" : "Inactive")
                        </MudChip>
                    </CellTemplate>
                </PropertyColumn>

                <!-- Actions column -->
                <TemplateColumn T="ProductDto" Title="Actions" Sortable="false" Filterable="false">
                    <CellTemplate>
                        <MudStack Row="true" Spacing="1">
                            <MudIconButton Icon="@Icons.Material.Filled.Visibility" 
                                          Size="Size.Small" 
                                          Color="Color.Info"
                                          OnClick="@(() => ViewProduct(context.Item.Id))" />
                            <MudIconButton Icon="@Icons.Material.Filled.Edit" 
                                          Size="Size.Small" 
                                          Color="Color.Primary"
                                          OnClick="@(() => EditProduct(context.Item.Id))" />
                            <MudIconButton Icon="@Icons.Material.Filled.Delete" 
                                          Size="Size.Small" 
                                          Color="Color.Error"
                                          OnClick="@(() => DeleteProduct(context.Item.Id))" />
                        </MudStack>
                    </CellTemplate>
                </TemplateColumn>
            </ChildContent>
        </CustomDataGrid>
    }
</MudContainer>

@code {
    private bool _loading = true;
    private List<ProductDto> _products = new();
    private string _searchString = string.Empty;
    private string _categoryFilter = "All";

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        try
        {
            _loading = true;
            _products = (await ProductService.GetAllAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading products: {ex.Message}", Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }

    private void ViewProduct(Guid id) => Navigation.NavigateTo($"/{modulename}/products/{id}");
    private void EditProduct(Guid id) => Navigation.NavigateTo($"/{modulename}/products/{id}/edit");

    private async Task DeleteProduct(Guid id)
    {
        // Add confirmation dialog and delete logic
        await ProductService.DeleteAsync(id);
        await LoadProducts();
        Snackbar.Add("Product deleted successfully", Severity.Success);
    }
}
```

**Important Notes:**
1. **@using directive:** Always add `@using {ModuleName}.Web.Components.Shared` at the top of your page to use CustomDataGrid
2. **TItem parameter:** Must match your DTO type (e.g., `TItem="ProductDto"`)
3. **QuickFilterFunc:** Provides instant client-side search across specified fields
4. **ToolbarContent:** Use for additional filters, dropdowns, or action buttons
5. **PropertyColumn vs TemplateColumn:** Use PropertyColumn for simple data display, TemplateColumn for custom rendering
6. **Don't use raw tables:** Avoid creating `<table>`, `<MudTable>`, or raw `<MudDataGrid>` - always use CustomDataGrid for consistency

**Example: Pro

```razor
@page "/{modulename}/products"
@using System.Net.Http.Json
@using MudBlazor
@inject IHttpClientFactory HttpClientFactory

<PageTitle>Products</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- Breadcrumbs (see section 8.2) -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudLink Href="/{modulename}">{ModuleName}</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">Products</MudText>
    </div>

    <!-- Page Header -->
    <div class="d-flex justify-space-between align-center mb-4">
        <div>
            <MudText Typo="Typo.h4" GutterBottom="true">
                <MudIcon Icon="@Icons.Material.Filled.Inventory" Class="mr-2" />
                Products
            </MudText>
            <MudText Typo="Typo.body1" Color="Color.Secondary">
                Manage your product catalog, SKUs, and pricing
            </MudText>
        </div>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add">
            Add Product
        </MudButton>
    </div>

    <!-- Loading / Error / Data States -->
    @if (_loading)
    {
        <MudProgressCircular Indeterminate="true" />
    }
    else if (!string.IsNullOrEmpty(_errorMessage))
    {
        <MudAlert Severity="Severity.Error">@_errorMessage</MudAlert>
    }
    else
    {
        <MudCard>
            <MudCardContent>
                <MudTable Items="@_products" Hover="true" Breakpoint="Breakpoint.Sm" Dense="true">
                    <HeaderContent>
                        <MudTh>SKU</MudTh>
                        <MudTh>Name</MudTh>
                        <MudTh>Price</MudTh>
                        <MudTh>Stock</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd DataLabel="SKU">@context.SKU</MudTd>
                        <MudTd DataLabel="Name">@context.Name</MudTd>
                        <MudTd DataLabel="Price">@context.Price.ToString("C")</MudTd>
                        <MudTd DataLabel="Stock">@context.TotalStock</MudTd>
                    </RowTemplate>
                </MudTable>
            </MudCardContent>
        </MudCard>
    }
</MudContainer>

@code {
    private bool _loading = true;
    private List<ProductDto> _products = new();
    private string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("{ModuleName}Api");
            _products = await client.GetFromJsonAsync<List<ProductDto>>("api/{modulename}/products") ?? new();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error loading products: {ex.Message}";
            Console.WriteLine(_errorMessage);
        }
        finally
        {
            _loading = false;
        }
    }

    // Define DTOs locally or reference from Contracts assembly
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalStock { get; set; }
    }
}
```

**Critical Rules for Sub-Pages:**
1. **Always** use `IHttpClientFactory` - NEVER inject `HttpClient` directly
2. **Always** use the named client: `HttpClientFactory.CreateClient("{ModuleName}Api")`
3. **Always** use relative API paths (e.g., `"api/inventory/products"`)
4. **Always** include loading, error, and data states
5. **Always** follow breadcrumb/padding conventions from section 8.2
6. **Always** add meaningful error messages for debugging

---

#### 8.3.2 Configure Web Program.cs
**IMPORTANT:** {ModuleName}.Web is a **standalone Blazor Web App** that:
- Runs on its own port (e.g., 5008 for Finance, 5002 for HR)
- Can be launched independently for testing
- Also gets embedded/referenced by the main shell (BusinessAsUsual.Web) for integrated navigation

Update `{ModuleName}.Web/Program.cs`:

```csharp
using {ModuleName}.Application.Services;
using {ModuleName}.Domain.Interfaces;
using {ModuleName}.Infrastructure.Persistence;
using {ModuleName}.Infrastructure.Repositories;
using {ModuleName}.Web.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Database configuration - use in-memory for standalone mode
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
	Console.WriteLine("⚠️  {ModuleName}.Web using in-memory database");
	builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
		options.UseInMemoryDatabase("{ModuleName}_Web"));
}
else
{
	var connectionString = builder.Configuration.GetConnectionString("{ModuleName}Database") 
		?? "Server=localhost;Database=BusinessAsUsual_{ModuleName};Trusted_Connection=True;TrustServerCertificate=True;";
	builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
		options.UseSqlServer(connectionString));
}

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Add other repositories...

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
// Add other services...

// Named HTTP client for API calls (optional, if Web needs to call API)
var apiUrl = builder.Configuration["{ModuleName}Service:Url"] ?? "http://localhost:50XX";
builder.Services.AddHttpClient("{ModuleName}Api", client =>
{
	client.BaseAddress = new Uri(apiUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
```

#### 8.4 Configure Web launchSettings.json
Create `{ModuleName}.Web/Properties/launchSettings.json`:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
	"http": {
	  "commandName": "Project",
	  "dotnetRunMessages": true,
	  "launchBrowser": false,
	  "applicationUrl": "http://localhost:50XX",
	  "environmentVariables": {
		"ASPNETCORE_ENVIRONMENT": "Development"
	  }
	}
  }
}
```

**Port Assignment Guide:**
- 5000: Main shell (BusinessAsUsual.Web)
- 5001: Finance.API
- 5002: HR.Web
- 5003: CRM.Web
- 5004: CRM.API
- 5008: Finance.Web
- 5041: HR.API
- 5100: ModuleRegistry.API
- Choose an available port in the 5000-5100 range for your module

#### 8.5 Configure Web Project File
Ensure `{ModuleName}.Web/{ModuleName}.Web.csproj` has:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <ItemGroup>
	<ProjectReference Include="..\{ModuleName}.Application\{ModuleName}.Application.csproj" />
	<ProjectReference Include="..\{ModuleName}.Infrastructure\{ModuleName}.Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
	<PackageReference Include="MudBlazor" Version="9.6.0" />
  </ItemGroup>

  <PropertyGroup>
	<TargetFramework>net9.0</TargetFramework>
	<Nullable>enable</Nullable>
	<ImplicitUsings>enable</ImplicitUsings>
	<StaticWebAssetBasePath>_content/{ModuleName}.Web</StaticWebAssetBasePath>
  </PropertyGroup>

  <ItemGroup>
	<!-- Exclude bootstrap from static web assets to avoid conflicts with parent shell -->
	<Content Remove="wwwroot\lib\bootstrap\**" />
	<None Include="wwwroot\lib\bootstrap\**">
	  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
	</None>
  </ItemGroup>

</Project>
```

⚠️ **Key Settings:**
- `Sdk="Microsoft.NET.Sdk.Web"` - NOT Razor SDK!
- `StaticWebAssetBasePath` - prevents asset conflicts when embedded in shell
- Bootstrap exclusion - prevents duplicate static files when referenced by main app

---

### Phase 9: Frontend Integration

#### 9.1 Add Web Project Reference to Main Shell
Add your module's Web project to `frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj`:

```xml
<ItemGroup>
  <!-- ... existing references ... -->
  <ProjectReference Include="..\..\services\Finance\Finance.Web\Finance.Web.csproj" />
  <ProjectReference Include="..\..\services\{ModuleName}\{ModuleName}.Web\{ModuleName}.Web.csproj" />
</ItemGroup>
```

Also add to the publish exclusion filter in the same file:

```xml
<Target Name="RemoveDuplicateReferencedWebContent" AfterTargets="ComputeFilesToPublish">
  <ItemGroup>
    <ResolvedFileToPublish Remove="@(ResolvedFileToPublish)"
      Condition="( $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/HR.Web/'))
                   Or $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/CRM.Web/'))
                   Or $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/Finance.Web/'))
                   Or $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/{ModuleName}.Web/')) )
                 And ( $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/wwwroot/'))
                       Or $([System.String]::Copy('%(Filename)').StartsWith('appsettings')) )" />
  </ItemGroup>
</Target>
```

This prevents duplicate wwwroot/appsettings files during publish.

#### 9.2 Update ModuleDiscoveryService Fallback
Add your module to `frontend/BusinessAsUsual.Web/Services/ModuleDiscoveryService.cs` in the `GetFallbackModules()` method:

```csharp
new ModuleDto
{
	ModuleId = "{modulename}",
	Key = "{modulename}",
	DisplayName = "{ModuleName}",
	Description = "Module description",
	UiEntryPoint = "/{modulename}",
	Icon = Icons.Material.Filled.Dashboard, // Choose appropriate icon
	IsActive = true,
	NavigationItems = new List<NavigationItemDto>
	{
		new() { Label = "Dashboard", Route = "/{modulename}", Icon = Icons.Material.Filled.Dashboard },
		new() 
		{ 
			Label = "Group Name", 
			Route = "/{modulename}/path", 
			Icon = Icons.Material.Filled.Inventory,
			ExpandedByDefault = false,
			Children = new List<NavigationItemDto>
			{
				new() { Label = "Submenu 1", Route = "/{modulename}/path1", Icon = Icons.Material.Filled.List },
				new() { Label = "Submenu 2", Route = "/{modulename}/path2", Icon = Icons.Material.Filled.Add }
			}
		},
		new() { Label = "Reports", Route = "/{modulename}/reports", Icon = Icons.Material.Filled.Analytics }
	}
}
```

#### 9.2.1 🔥 CRITICAL: Register Module Route in MainLayout.razor.cs

**Problem:** Even if the module is in the sidebar and AdditionalAssemblies, the **sidebar won't appear** when you navigate to the module because `_currentModule` stays null.

**Why:** The shell's `MainLayout.razor.cs` has hardcoded module route detection that needs updated for each new module.

**Solution:** Add your module to the legacy route detection in `frontend/BusinessAsUsual.Web/Components/Layout/MainLayout.razor.cs`:

Find the `UpdateModuleFromUri` method (around line 182) and add your module:

```csharp
// Legacy hardcoded routes
if (path.StartsWith("/hr"))
	_currentModule = "HR";
else if (path.StartsWith("/finance"))
	_currentModule = "Finance";
else if (path.StartsWith("/crm"))
	_currentModule = "CRM";
else if (path.StartsWith("/{modulename}"))
	_currentModule = "{ModuleName}";  // ← ADD THIS LINE
else if (path.StartsWith("/timekeeping"))
	_currentModule = "Timekeeping";
// ... rest of the conditions
```

**Validation:** After this change, navigating to `/{modulename}` should show the sidebar with your module's navigation.

**Note:** The `_currentModule` string must match your module's `DisplayName` from `ModuleDiscoveryService.cs`.

#### 9.3 Add to Visual Studio Solution (if not done in Step 2.2)
If you haven't already added projects to the solution:

Right-click solution → Add → Existing Project, add all 7 projects.

⚠️ **Double-check:** All projects should appear in Solution Explorer. If any are missing, they weren't added correctly in step 2.2.

#### 9.4 Configure Multiple Startup Projects
**CRITICAL:** Set up multi-project startup so all services run together:

1. Right-click solution → **Properties**
2. Select **Multiple startup projects**
3. Set **Action = Start** for:
   - `ModuleRegistry.API`
   - `{ModuleName}.API`
   - `{ModuleName}.Web` ← **Important!** This allows standalone testing
   - `BusinessAsUsual.Web` (main shell)
   - Any other module APIs you need running (Finance.API, HR.API, etc.)

4. **Click OK**

Now pressing F5 will start all projects together!

**Testing Modes:**
- **Integrated:** Navigate to `http://localhost:5000` (main shell) and click your module in the sidebar
- **Standalone:** Navigate directly to `http://localhost:50XX` (your Web app's port) to test in isolation

---

### Phase 10: Testing & Validation

#### 10.1 Create Unit Tests
Create in `{ModuleName}.Tests/Unit/`:

```csharp
using Xunit;
using Moq;
using {ModuleName}.Application.Services;
using {ModuleName}.Domain.Interfaces;

public class ProductServiceTests
{
	[Fact]
	public async Task GetAllAsync_ReturnsAllProducts()
	{
		// Arrange
		var mockRepo = new Mock<IProductRepository>();
		mockRepo.Setup(r => r.GetAllAsync())
			.ReturnsAsync(new List<Product> { /* test data */ });

		var service = new ProductService(mockRepo.Object);

		// Act
		var result = await service.GetAllAsync();

		// Assert
		Assert.NotNull(result);
		mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
	}
}
```

#### 10.2 Validation Checklist
- [ ] API starts without errors
- [ ] Database migrations apply successfully
- [ ] Module registers with Module Registry
- [ ] Navigation appears in sidebar
- [ ] Mobile UI contract endpoint returns valid JSON
- [ ] CRUD operations work correctly
- [ ] Dashboard loads with data
- [ ] All navigation links work
- [ ] Build succeeds with no warnings
- [ ] Unit tests pass

---

## Port Numbers Reference
Assign unique ports to avoid conflicts:

| Module | API Port | Web Port |
|--------|----------|----------|
| ModuleRegistry | 5100 | - |
| Finance | 5007 | 5008 |
| CRM | 5004 | 5003 |
| HR | 5041 | 5042 |
| **{New Module}** | **50XX** | **50XX** |

---

## Common Material Icons
```csharp
Icons.Material.Filled.Dashboard
Icons.Material.Filled.Inventory
Icons.Material.Filled.ShoppingCart
Icons.Material.Filled.LocalShipping
Icons.Material.Filled.Campaign
Icons.Material.Filled.Support
Icons.Material.Filled.Build
Icons.Material.Filled.Assignment
Icons.Material.Filled.Description
Icons.Material.Filled.VerifiedUser
Icons.Material.Filled.Warehouse
Icons.Material.Filled.Category
Icons.Material.Filled.Store
Icons.Material.Filled.Sell
```

---

## Common Issues & Troubleshooting

### Build Failures

#### Issue: "Program does not contain a static 'Main' method"
**Cause:** Project SDK mismatch or missing Program.cs  
**Fix:** 
- Ensure `.csproj` has `<Project Sdk="Microsoft.NET.Sdk.Web">` (NOT Razor SDK)
- Verify `Program.cs` exists in the Web project

#### Issue: "CS0246: The type or namespace name 'HttpContext' could not be found"
**Cause:** Standalone Blazor app trying to use ASP.NET Core types  
**Fix:** Remove or conditionally compile code that uses `HttpContext` in component libraries

#### Issue: "Conflicting assets with the same target path 'lib/bootstrap/...'"
**Cause:** Multiple Web projects shipping the same wwwroot files  
**Fix:** 
1. Add your module to the exclusion filter in `BusinessAsUsual.Web.csproj`
2. Add bootstrap exclusion to your Web project (see step 8.4)

#### Issue: Package downgrade warning (e.g., MudBlazor 9.7.0 to 9.6.0)
**Cause:** Version mismatch between main app and module  
**Fix:** Use MudBlazor 9.6.0 in all Web projects to match the shell

#### Issue: "The type or namespace name 'EF/OpenApi/etc.' could not be found"
**Cause:** Using .NET 10 packages with .NET 9 project  
**Fix:** 
```bash
# Downgrade to 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.0
```

#### Issue: Projects restore as net10.0 even though .csproj says net9.0
**Cause:** Template defaults or cached restore  
**Fix:**
1. Manually edit each `.csproj` to verify `<TargetFramework>net9.0</TargetFramework>`
2. Clean and rebuild: `dotnet clean && dotnet build`

### Runtime/Navigation Issues

#### Issue: "Nothing at this address" when clicking module link
**Possible Causes & Fixes:**

1. **Web project not added to shell reference**
   - Add `<ProjectReference Include="..\..\services\{ModuleName}\{ModuleName}.Web\{ModuleName}.Web.csproj" />` to `BusinessAsUsual.Web.csproj`

2. **Web project not in startup projects**
   - Configure Multiple Startup Projects (see step 9.4)
   - Set {ModuleName}.Web action to "Start"

3. **Route mismatch**
   - Verify `@page "/{modulename}"` in Dashboard.razor matches the route in ModuleDiscoveryService fallback navigation

4. **Web app not running**
   - Check Output window for startup errors
   - Verify port is not already in use

#### Issue: Module doesn't appear in sidebar
**Cause:** Missing from ModuleDiscoveryService fallback  
**Fix:** Add module entry to `GetFallbackModules()` in `ModuleDiscoveryService.cs` (see step 9.2)

#### Issue: Module appears in sidebar but clicking does nothing
**Cause:** Missing `@using MudBlazor` in Razor pages  
**Fix:** Add `@using MudBlazor` to each `.razor` file, or add to `_Imports.razor`

#### Issue: Sidebar menu items stay expanded instead of collapsed
**Cause:** Missing `ExpandedByDefault = false` on navigation groups  
**Fix:** Add `ExpandedByDefault = false` to parent navigation items with children

#### Issue: Dashboard shows stats/alerts but no navigation cards
**Cause:** Module dashboard missing navigation card section  
**Fix:** Add a grid with MudCards linking to each major module feature (Products, Warehouses, etc.). See Inventory Dashboard.razor for example.

#### Issue: No sidebar visible when navigating to module pages
**Possible Causes & Fixes:**

1. **Wrong browser URL/port**
   - **Symptom:** You see topbar/footer but no sidebar; page works but feels "isolated"
   - **Cause:** Browser navigated to module's standalone port (e.g., `localhost:5009`) instead of shell port
   - **Fix:** Always navigate through the shell app. Check the URL bar - it should be the shell's port (typically 5001 or 5000), not the module's port.
   - **Prevention:** Don't bookmark or directly visit module standalone URLs when developing integrated features

2. **Module using its own MainLayout**
   - **Symptom:** Module pages render with different layout than other modules
   - **Cause:** Module's App.razor or pages explicitly specify layout
   - **Fix:** Module pages should NOT specify `@layout` directive. They should inherit the shell's MainLayout automatically when loaded via the shell router.
   - **Note:** Module can have its own MainLayout for standalone development, but pages should not force it

3. **Module not properly registered in shell router**
   - **Fix:** Verify module assembly is in shell's `App.razor` `AdditionalAssemblies` array

**Best Practice:** When developing module features, always access them through the shell sidebar navigation, not by typing the module's standalone URL directly.

### Visual Studio Issues

#### Issue: Module not appearing in startup project list
**Cause:** Projects not added to solution  
**Fix:** 
```bash
cd "D:\DotNet Projects\BusinessAsUsual"
dotnet sln add services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj
# ... repeat for all 7 projects
```

#### Issue: "Project not found" error when adding reference
**Cause:** Wrong relative path  
**Fix:** Always use `../{ProjectName}/{ProjectName}.csproj` for sibling projects

### Database/EF Core Issues

#### Issue: Migrations fail to create
**Cause:** Missing EF tools or wrong startup project  
**Fix:**
```bash
dotnet tool install --global dotnet-ef
cd services/{ModuleName}/{ModuleName}.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../{ModuleName}.API
```

#### Issue: "No database provider configured"
**Cause:** DbContext not registered in DI  
**Fix:** Verify `builder.Services.AddDbContext<{ModuleName}DbContext>()` exists in both API and Web `Program.cs`

### MudBlazor/Blazor Issues

#### Issue: "Generic type 'MudList<T>' requires 1 type argument"
**Cause:** MudBlazor version incompatibility  
**Fix:** Replace `MudList<MudListItem>` with simpler structures like `MudStack` + `MudButton`

#### Issue: "Cannot infer type for 'T' in MudChip"
**Cause:** Missing explicit type parameter  
**Fix:** Change `<MudChip>` to `<MudChip T="string">`

#### Issue: Icons.Material.Filled not recognized
**Cause:** Missing MudBlazor using  
**Fix:** Add `@using MudBlazor` at top of .razor file

### Module Registration Issues

#### Issue: Module shows as offline in registry
**Cause:** Registration service not running or wrong URL  
**Fix:** 
1. Verify ModuleRegistry.API is running on port 5100
2. Check `appsettings.json` has correct `ModuleRegistry:Url`
3. Check API Output window for registration errors

#### Issue: Mobile UI contract returns 404
**Cause:** Missing MobileUIController or wrong route  
**Fix:** Ensure controller has `[Route("api/[controller]")]` and `[HttpGet("navigation")]` attributes

---

## Tips & Best Practices

1. **Naming Conventions**: Use PascalCase for projects, classes; camelCase for fields/parameters
2. **DTOs vs Entities**: Always map between them, never expose entities directly
3. **Async/Await**: Use async methods for all I/O operations
4. **Error Handling**: Return appropriate HTTP status codes (404, 400, 500)
5. **Logging**: Add logging to services for debugging
6. **Validation**: Use Data Annotations or FluentValidation
7. **Navigation**: Keep menu structure flat (max 2 levels deep)
8. **Mobile Contracts**: Update both registration and mobile UI controller
9. **Testing**: Write unit tests for business logic, integration tests for APIs
10. **Documentation**: Add XML comments to public methods

---

### Phase 11: Docker Deployment Configuration

After creating and testing your module locally, add Docker support for containerized deployment.

#### 11.1 Create Dockerfile for API
Create `services/{ModuleName}/{ModuleName}.API/Dockerfile`:

```dockerfile
# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the {ModuleName} dependency graph) for a cached restore
COPY services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj services/{ModuleName}/{ModuleName}.API/
COPY services/{ModuleName}/{ModuleName}.Application/{ModuleName}.Application.csproj services/{ModuleName}/{ModuleName}.Application/
COPY services/{ModuleName}/{ModuleName}.Contracts/{ModuleName}.Contracts.csproj services/{ModuleName}/{ModuleName}.Contracts/
COPY services/{ModuleName}/{ModuleName}.Domain/{ModuleName}.Domain.csproj services/{ModuleName}/{ModuleName}.Domain/
COPY services/{ModuleName}/{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj services/{ModuleName}/{ModuleName}.Infrastructure/

# Restore
# Retry the isolated restore to survive transient NuGet connectivity on small
# EC2 instances (a flaky network otherwise fails restore with exit code 82).
RUN for i in 1 2 3; do \
	  dotnet restore services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj && break; \
	  if [ "$i" = "3" ]; then echo "restore failed after 3 attempts"; exit 1; fi; \
	  echo "restore attempt $i failed; retrying in 10s..."; sleep 10; \
	done

# Copy source
COPY services/{ModuleName}/{ModuleName}.API services/{ModuleName}/{ModuleName}.API
COPY services/{ModuleName}/{ModuleName}.Application services/{ModuleName}/{ModuleName}.Application
COPY services/{ModuleName}/{ModuleName}.Contracts services/{ModuleName}/{ModuleName}.Contracts
COPY services/{ModuleName}/{ModuleName}.Domain services/{ModuleName}/{ModuleName}.Domain
COPY services/{ModuleName}/{ModuleName}.Infrastructure services/{ModuleName}/{ModuleName}.Infrastructure

# Publish
RUN dotnet publish services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# curl is used by the container HEALTHCHECK below
RUN apt-get update && apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
	CMD curl -fsS http://localhost:80/health || exit 1

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "{ModuleName}.API.dll"]
```

**Key Features:**
- **Multi-stage build** - Smaller final image (SDK vs Runtime)
- **Retry logic** - Handles transient NuGet failures
- **Health check** - Monitors container health
- **Dependency order** - Copies projects in dependency graph order for efficient layer caching

#### 11.2 Create Dockerfile for Web
Create `services/{ModuleName}/{ModuleName}.Web/Dockerfile`:

```dockerfile
# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the {ModuleName} dependency graph) for a cached restore
COPY services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj services/{ModuleName}/{ModuleName}.Web/
COPY services/{ModuleName}/{ModuleName}.Application/{ModuleName}.Application.csproj services/{ModuleName}/{ModuleName}.Application/
COPY services/{ModuleName}/{ModuleName}.Domain/{ModuleName}.Domain.csproj services/{ModuleName}/{ModuleName}.Domain/
COPY services/{ModuleName}/{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj services/{ModuleName}/{ModuleName}.Infrastructure/

# Restore
RUN dotnet restore services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj

# Copy source
COPY services/{ModuleName}/{ModuleName}.Web services/{ModuleName}/{ModuleName}.Web
COPY services/{ModuleName}/{ModuleName}.Application services/{ModuleName}/{ModuleName}.Application
COPY services/{ModuleName}/{ModuleName}.Domain services/{ModuleName}/{ModuleName}.Domain
COPY services/{ModuleName}/{ModuleName}.Infrastructure services/{ModuleName}/{ModuleName}.Infrastructure

# Publish
RUN dotnet publish services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "{ModuleName}.Web.dll"]
```

#### 11.3 Test Docker Build Locally
Before pushing, test the Docker builds locally:

```bash
# Build API image
docker build -f services/{ModuleName}/{ModuleName}.API/Dockerfile -t {modulename}-api:latest .

# Build Web image
docker build -f services/{ModuleName}/{ModuleName}.Web/Dockerfile -t {modulename}-web:latest .

# Run API container
docker run -d -p 5007:80 --name {modulename}-api {modulename}-api:latest

# Run Web container
docker run -d -p 5008:80 --name {modulename}-web {modulename}-web:latest

# Test the containers
curl http://localhost:5007/health
curl http://localhost:5008

# Clean up
docker stop {modulename}-api {modulename}-web
docker rm {modulename}-api {modulename}-web
```

#### 11.4 Add Module to docker-compose.heavy.yml
After creating the Dockerfiles, add your module services to the orchestration file `docker-compose.heavy.yml`:

```yaml
  {modulename}-api:
    build:
      context: .
      dockerfile: services/{ModuleName}/{ModuleName}.API/Dockerfile
    image: bau/{modulename}-api:latest
    container_name: bau-{modulename}-api
    restart: unless-stopped
    networks:
      - bau-heavy
    ports:
      - "50XX:80"  # Choose an available port number
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Production}
      - ASPNETCORE_URLS=http://+:80
      # Shared RDS (MS SQL) connection string - set in .env on the instance
      - AWS_SQL_CONNECTION_STRING=${AWS_SQL_CONNECTION_STRING}
      # Module Registry URL for service discovery
      - ModuleRegistry__Url=${MODULE_REGISTRY_URL}
    deploy:
      resources:
        limits:
          memory: 512M

  {modulename}-web:
    build:
      context: .
      dockerfile: services/{ModuleName}/{ModuleName}.Web/Dockerfile
    image: bau/{modulename}-web:latest
    container_name: bau-{modulename}-web
    restart: unless-stopped
    networks:
      - bau-heavy
    ports:
      - "50YY:80"  # Choose an available port number
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Production}
      - ASPNETCORE_URLS=http://+:80
      # Module Web calls Module API over the internal compose network
      - {ModuleName}Api__Url=http://{modulename}-api:80
    depends_on:
      - {modulename}-api
    deploy:
      resources:
        limits:
          memory: 384M
```

**Port Assignments (update header comment too):**
- CRM: 5004 (API), 5005 (Web)
- HR: 5041 (API), 5002 (Web)
- Finance: 5006 (API), 5009 (Web)
- Inventory: 5010 (API), 5011 (Web)
- **Your module:** Choose the next available port numbers

**Important:**
- Add your module to the header comment section listing all services
- Ensure port numbers don't conflict with existing services
- Memory limits: 512M for API, 384M for Web (typical for module services)
- Use the internal Docker network name for inter-service communication

**Critical Configuration Alignment:**
The environment variable name in docker-compose MUST match the configuration key in your Web project's Program.cs:

```yaml
# docker-compose.heavy.yml
environment:
  - {ModuleName}Api__Url=http://{modulename}-api:80
  # Note: Double underscore (__) in environment variable = colon (:) in .NET configuration
```

```csharp
// {ModuleName}.Web/Program.cs
var apiUrl = builder.Configuration["{ModuleName}Api:Url"] ?? "http://localhost:50XX";
builder.Services.AddHttpClient("{ModuleName}Api", client =>
{
    client.BaseAddress = new Uri(apiUrl);
});
```

**Common Mistake:**
```csharp
// ❌ WRONG - Key mismatch
var apiUrl = builder.Configuration["{ModuleName}Service:Url"] ?? "http://localhost:50XX";
// Docker sets "{ModuleName}Api__Url" but code reads "{ModuleName}Service:Url"
// Result: Cannot assign requested address (localhost:50XX) error in Docker
```

```csharp
// ✅ CORRECT - Keys match
var apiUrl = builder.Configuration["{ModuleName}Api:Url"] ?? "http://localhost:50XX";
// Docker sets "{ModuleName}Api__Url" which maps to "{ModuleName}Api:Url" in config
// Result: Web successfully calls http://{modulename}-api:80 over Docker network
```

**Pattern Examples:**
- Finance: `FinanceApi:Url` ← `FinanceApi__Url`
- CRM: `CrmApi:Url` ← `CrmApi__Url`
- Inventory: `InventoryApi:Url` ← `InventoryApi__Url`

#### 11.5 Update CI/CD Pipeline
If you have a CI/CD pipeline (GitHub Actions, Azure DevOps, etc.), add build steps for the new module's Docker images.

**Important Notes:**
- Dockerfiles must be run from the **solution root directory** (not from the module folder) because they reference relative paths
- The COPY commands expect the full relative path from the solution root
- Don't add Dockerfiles until the module is fully integrated and tested in the main shell

---

## Code Quality Standards

### Documentation Requirements

**CRITICAL:** All public properties, methods, and classes MUST have XML documentation comments.

#### ✅ Good Examples:

```csharp
/// <summary>
/// Represents a product in the inventory system.
/// </summary>
public class Product
{
	/// <summary>
	/// Gets or sets the unique identifier for the product.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the product name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the current stock quantity.
	/// </summary>
	public int QuantityOnHand { get; set; }
}

/// <summary>
/// Service for managing product inventory operations.
/// </summary>
public class ProductService : IProductService
{
	/// <summary>
	/// Retrieves all products from the inventory.
	/// </summary>
	/// <returns>A collection of all products.</returns>
	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		// Implementation
	}

	/// <summary>
	/// Creates a new product in the inventory.
	/// </summary>
	/// <param name="request">The product creation request containing product details.</param>
	/// <returns>The newly created product.</returns>
	/// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
	public async Task<ProductDto> CreateAsync(CreateProductRequest request)
	{
		// Implementation
	}
}
```

#### ❌ Bad Examples (Missing Documentation):

```csharp
// BAD - No documentation
public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; }
}

// BAD - No documentation on public methods
public class ProductService
{
	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		// ...
	}
}
```

#### Enable Documentation Warnings

Add to your `.csproj` files to enforce documentation:

```xml
<PropertyGroup>
	<GenerateDocumentationFile>true</GenerateDocumentationFile>
	<NoWarn>$(NoWarn);1591</NoWarn> <!-- Remove this to enforce doc comments -->
</PropertyGroup>
```

To enforce documentation and fail builds on missing comments, remove the `NoWarn` suppression.

---

## Lessons Learned: Deployment Troubleshooting

This section captures common issues encountered during deployment and their solutions.

### MudBlazor Attribute Casing Issues

**Problem:** Build warnings like `MUD0002: Illegal Attribute 'Title' on 'MudIconButton' using pattern 'LowerCase'`

**Cause:** MudBlazor analyzer enforces lowercase attribute names for standard HTML attributes.

**Solution:**
- Change `Title="..."` to `title="..."` on `MudIconButton`
- Change `PanelClass="..."` to `Class="..."` on `MudTabs`
- Change `Dense="true"` - Remove this attribute entirely from `MudTimePicker` (not supported)

**Example Fixes:**
```razor
<!-- BEFORE (Warning) -->
<MudIconButton Icon="@Icons.Material.Filled.Edit" Title="Edit" />
<MudTabs PanelClass="pa-4">...</MudTabs>

<!-- AFTER (Fixed) -->
<MudIconButton Icon="@Icons.Material.Filled.Edit" title="Edit" />
<MudTabs Class="pa-4">...</MudTabs>
```

### Missing Component References

**Problem:** Warning `RZ10012: Found markup element with unexpected name 'CustomDataGrid'. If this is intended to be a component, add a @using directive for its namespace.`

**Cause:** Razor compiler can't find the component because the namespace isn't imported.

**Solution:** Add `@using {ModuleName}.Web.Components.Shared` at the top of the .razor file.

```razor
@page "/hr/benefits"
@using HR.Web.Components.Shared  <!-- Add this line -->
@rendermode InteractiveServer

<!-- Now CustomDataGrid can be used -->
<CustomDataGrid TItem="BenefitDto" Items="@_benefits">
	...
</CustomDataGrid>
```

**Alternative:** Add to `_Imports.razor` to apply to all pages:
```razor
@using {ModuleName}.Web.Components.Shared
```

### Null Reference Warnings in Production Builds

**Problem:** Warning `CS8604: Possible null reference argument for parameter` in Release builds but not Debug.

**Cause:** Nullable reference types enabled, and the compiler detects potential null values being passed to non-nullable parameters.

**Solution:** Add null checks before using nullable variables:

```csharp
// BEFORE (Warning)
await Service.UpdateAsync(_config);

// AFTER (Fixed)
if (_config != null)
{
	await Service.UpdateAsync(_config);
}
```

**Best Practice:** Always check nullable variables before use, or use null-coalescing operators:
```csharp
var name = customer?.Name ?? "Unknown";
```

### Async Method Without Await

**Problem:** Warning `CS1998: This async method lacks 'await' operators and will run synchronously.`

**Cause:** Method is marked `async` but doesn't contain any `await` calls.

**Solution:** Either add async operations or use `Task.FromResult` for synchronous methods that need to return `Task<T>`:

```csharp
// BEFORE (Warning)
private async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchText)
{
	return _employees.Where(e => e.Name.Contains(searchText));
}

// AFTER (Fixed Option 1 - Use Task.FromResult)
private Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchText)
{
	return Task.FromResult(_employees.Where(e => e.Name.Contains(searchText)));
}

// AFTER (Fixed Option 2 - Remove async if not needed)
private IEnumerable<EmployeeDto> SearchEmployees(string searchText)
{
	return _employees.Where(e => e.Name.Contains(searchText));
}
```

### Module Reference in Docker Build Context

**Problem:** Error `CS0246: The type or namespace name 'Inventory' could not be found` during Docker build.

**Cause:** Module project isn't included in the Docker build context or hasn't been pushed to the repository yet.

**Solution (Temporary):** Comment out the module reference in `App.razor` and `.csproj` until the module is deployed:

```razor
<!-- App.razor -->
@code {
	private readonly Assembly[] _additionalAssemblies = new[]
	{
		typeof(HR.Web.Components.App).Assembly,
		typeof(CRM.Web.Components.App).Assembly,
		typeof(Finance.Web.Components.App).Assembly
		// typeof(Inventory.Web.Components.App).Assembly  // TODO: Re-enable when deployed
	};
}
```

```xml
<!-- BusinessAsUsual.Web.csproj -->
<ItemGroup>
	<ProjectReference Include="..\..\services\HR\HR.Web\HR.Web.csproj" />
	<ProjectReference Include="..\..\services\CRM\CRM.Web\CRM.Web.csproj" />
	<ProjectReference Include="..\..\services\Finance\Finance.Web\Finance.Web.csproj" />
	<!-- <ProjectReference Include="..\..\services\Inventory\Inventory.Web\Inventory.Web.csproj" /> -->
</ItemGroup>
```

**Permanent Solution:** Create Dockerfiles for the new module (see Phase 11) and include them in your deployment pipeline.

### Generic Type Inference in MudBlazor Components

**Problem:** Error `RZ10001: The type of component 'MudChip' cannot be inferred based on the values provided.`

**Cause:** Some MudBlazor components require explicit type parameter `T`.

**Solution:** Add `T="string"` (or appropriate type) to the component:

```razor
<!-- BEFORE (Error) -->
<MudChip Size="Size.Small" Color="Color.Primary">New</MudChip>

<!-- AFTER (Fixed) -->
<MudChip T="string" Size="Size.Small" Color="Color.Primary">New</MudChip>
```

### Build vs. Runtime Environment Differences

**Problem:** Code builds fine locally but fails in Docker/CI pipeline.

**Common Causes:**
1. **Case-sensitive file paths** - Linux containers are case-sensitive, Windows is not
2. **Missing files** - .gitignore might exclude necessary files
3. **Environment-specific configuration** - Missing environment variables or connection strings
4. **Package restore issues** - Network timeouts in CI environment

**Solutions:**
1. Always use correct casing in file references: `"MyFile.cs"` not `"myfile.cs"`
2. Check `.gitignore` and `.dockerignore` for excluded files
3. Use `appsettings.Production.json` for deployment-specific settings
4. Add retry logic to Docker restores (see Phase 11.1 Dockerfile example)

### Testing Checklist Before Deployment

Before pushing changes that will trigger a deployment pipeline:

- [ ] All projects build successfully in **Release** configuration
- [ ] No compiler warnings (treat warnings as errors in production)
- [ ] All unit tests pass
- [ ] Manual testing completed in integrated mode (via shell)
- [ ] All public members have XML doc comments
- [ ] No `TODO` or `HACK` comments in committed code
- [ ] appsettings.Production.json configured (if needed)
- [ ] Dockerfiles created and tested locally (Phase 11)
- [ ] Module properly registered in shell references

**Build Release Mode Locally:**
```bash
dotnet build -c Release
dotnet test -c Release
```

### AWS Deployment: Disk Space Management

**Problem:** Docker build fails with `No space left on device` error during NuGet restore or file operations.

**Cause:** The default 8GB EBS volume is too small for building multiple .NET 9 microservices simultaneously. Each module's Docker build can consume 1-2GB during the build process (NuGet packages, intermediate layers, build cache).

**Example Error:**
```
error : No space left on device : '/root/.nuget/packages/...'
System.IO.IOException: No space left on device
```

**Immediate Cleanup (Temporary Fix):**
```sh
# Connect to your EC2 instance
# Check current disk usage
df -h /

# Stop all containers
sudo docker compose -f docker-compose.heavy.yml down

# Remove all unused Docker artifacts
sudo docker system prune -af --volumes

# Check space freed
df -h /
```

This typically frees 2-4GB but is only a temporary solution.

**Permanent Solution - Resize EBS Volume:**

You MUST resize the EBS volume to at least 20-30GB for sustainable operation with 4+ modules.

**Step 1: Resize in AWS Console**
1. Navigate to **EC2 Console** → **Volumes**
2. Select the volume attached to your heavy instance (currently 8 GiB)
3. **Actions** → **Modify Volume**
4. Change **Size** from `8` to `30` (30 GB recommended)
5. Click **Modify** → **Yes** to confirm
6. Wait 2-5 minutes for state to change to "optimizing"

**Step 2: Extend the Filesystem (on the EC2 instance)**

After AWS shows the volume as modified, SSH to your instance and run:

```sh
# Extend the partition to use new space
sudo growpart /dev/nvme0n1 1

# For XFS filesystem (Amazon Linux 2023 default)
sudo xfs_growfs /

# For ext4 filesystem (older AMIs)
sudo resize2fs /dev/nvme0n1p1

# Verify the new size is available
df -h /
```

You should now see ~30GB total instead of 8GB.

**Step 3: Rebuild Docker Containers**

```sh
# Navigate to repo (adjust path if different)
cd /home/ec2-user/BusinessAsUsual

# Pull latest code
git pull

# Rebuild and start all services
sudo docker compose -f docker-compose.heavy.yml up -d --build
```

**Why 30GB?**
- **8GB**: Too small - fills up during multi-service builds ❌
- **20GB**: Minimum for 4 modules, tight headroom ⚠️
- **30GB**: Recommended - room for logs, cache, future modules ✅
- **Cost**: ~$3/month for 30GB gp3 volume (minimal increase from 8GB)

**Important Notes:**
- Docker builds all services in parallel by default, consuming disk space simultaneously
- NuGet package cache (`/root/.nuget/packages/`) can grow to several GB
- Docker layer cache improves rebuild speed but consumes disk space
- Regular `docker system prune -af` cleanup is still recommended monthly

**User Account Context:**
- You may SSH as `ssm-user` (AWS Systems Manager) or `ec2-user`
- Repository is typically located at `/home/ec2-user/BusinessAsUsual`
- Docker commands require `sudo` privileges
- To switch users: `sudo su - ec2-user`
- To run commands as another user: Use full paths with sudo (e.g., `sudo docker compose -f /home/ec2-user/BusinessAsUsual/docker-compose.heavy.yml up -d`)

---

## Next Steps After Creation

1. Add authentication & authorization
2. Implement role-based permissions
3. Add audit logging
4. Create reports and analytics
5. Implement search and filtering
6. Add export functionality (Excel, PDF)
7. Create mobile app screens
8. Add real-time notifications
9. Implement webhooks/integrations
10. Performance optimization

---

## Example: Quick Inventory Module Creation

When you say "Let's create an Inventory module", this skill will:
1. ✅ Confirm the domain (Inventory Management)
2. ✅ Design entities (Product, Warehouse, StockItem, etc.)
3. ✅ Create 7 projects following the structure
4. ✅ Implement domain layer with entities
5. ✅ Build infrastructure with EF Core
6. ✅ Create application services
7. ✅ Build REST API with controllers
8. ✅ Register with Module Registry
9. ✅ Create mobile contracts
10. ✅ Build Blazor dashboard and pages
11. ✅ Integrate into main app navigation
12. ✅ Validate everything works

**Estimated time**: 2-4 hours for a complete, functional module

---

## Questions to Ask Before Starting
- What is the module name?
- What are the 3-5 core entities?
- What's the primary workflow/user journey?
- Should it integrate with existing modules?
- Mobile support required?
- Any special compliance/security requirements?
