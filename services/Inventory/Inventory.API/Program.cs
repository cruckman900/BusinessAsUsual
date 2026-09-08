using Inventory.Application.Services;
using Inventory.API.Seeding;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using BusinessAsUsual.Application.Services;
using BusinessAsUsual.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use in-memory database for development
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
    Console.WriteLine("⚠️  Inventory.API using in-memory database");
    builder.Services.AddDbContext<InventoryDbContext>(options =>
        options.UseInMemoryDatabase("Inventory_API"));
}
else
{
    builder.Services.AddDbContext<InventoryDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryConnection")));
}

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IStockItemRepository, StockItemRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();

// Register tenant context for multi-tenant isolation
builder.Services.AddScoped<ITenantContext, TenantContext>();
// Singleton AsyncLocal-backed accessor so TenantPropagationHandler (constructed in an
// IHttpClientFactory-managed scope) can safely read the current request's tenant without
// depending on the Scoped ITenantContext directly. See TenantContextAccessor for rationale.
builder.Services.AddSingleton<ITenantContextAccessor, TenantContextAccessor>();

// Register application services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<PurchaseOrderService>();
builder.Services.AddScoped<SupplierService>();

// Register event bus for cross-module integration events
builder.Services.AddInProcessEventBus();

// Register event handlers
builder.Services.AddScoped<IIntegrationEventHandler<OrderShippedIntegrationEvent>, Inventory.API.EventHandlers.OrderShippedEventHandler>();

// Register HTTP client for module registration
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();

// Keep the module registered (retry on startup + heartbeat to survive registry restarts)
builder.Services.AddHostedService<Inventory.API.Services.ModuleRegistrationHostedService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Seed in-memory database with test data
if (useInMemory)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        await new InventorySeeder(context).SeedAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Tenant resolution middleware - must come before controllers so repositories can filter by CompanyId
app.UseMiddleware<TenantResolutionMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymousTenant();

// Dev/demo-only endpoint that clears and reseeds the in-memory Inventory database so a
// fresh, fully-populated "tenant" dataset is available every time a user logs in with
// the admin/password demo credentials.
app.MapPost("/api/inventory/tenant-reset", async (InventoryDbContext context) =>
{
    if (!useInMemory)
    {
        return Results.BadRequest("Tenant reset is only supported for the in-memory demo database.");
    }

    context.PurchaseOrderLines.RemoveRange(context.PurchaseOrderLines);
    context.PurchaseOrders.RemoveRange(context.PurchaseOrders);
    context.CycleCounts.RemoveRange(context.CycleCounts);
    context.StockTransfers.RemoveRange(context.StockTransfers);
    context.StockAdjustments.RemoveRange(context.StockAdjustments);
    context.InventoryTransactions.RemoveRange(context.InventoryTransactions);
    context.StockItems.RemoveRange(context.StockItems);
    context.BinLocations.RemoveRange(context.BinLocations);
    context.Products.RemoveRange(context.Products);
    context.Suppliers.RemoveRange(context.Suppliers);
    context.Warehouses.RemoveRange(context.Warehouses);
    await context.SaveChangesAsync();

    await new InventorySeeder(context).SeedAsync();

    return Results.Ok(new { message = "Inventory tenant database reset and reseeded." });
}).AllowAnonymousTenant();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }

