using Sales.API.Seeding;
using Sales.Application.Services;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Persistence;
using Sales.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Application.Services;
using BusinessAsUsual.Infrastructure.Middleware;
using BusinessAsUsual.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use in-memory database for development
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
    Console.WriteLine("⚠️  Sales.API using in-memory database");
    builder.Services.AddDbContext<SalesDbContext>(options =>
        options.UseInMemoryDatabase("Sales_API"));
}
else
{
    builder.Services.AddDbContext<SalesDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SalesConnection")));
}

// Register repositories
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register tenant context for multi-tenant isolation
builder.Services.AddScoped<ITenantContext, TenantContext>();
// Singleton AsyncLocal-backed accessor so TenantPropagationHandler (constructed in an
// IHttpClientFactory-managed scope) can safely read the current request's tenant without
// depending on the Scoped ITenantContext directly. See TenantContextAccessor for rationale.
builder.Services.AddSingleton<ITenantContextAccessor, TenantContextAccessor>();

// Register application services
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Register event bus for cross-module integration events
builder.Services.AddInProcessEventBus();

// Register tenant-aware HTTP clients used for cross-module seed data fetches
builder.Services.AddTransient<TenantPropagationHandler>();
builder.Services.AddHttpClient("InventoryApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Inventory:ApiBaseUrl"] ?? "http://localhost:5142");
}).AddHttpMessageHandler<TenantPropagationHandler>();
builder.Services.AddHttpClient("CrmApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Crm:ApiBaseUrl"] ?? "http://localhost:5004");
}).AddHttpMessageHandler<TenantPropagationHandler>();

// Register HTTP client for module registration
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();

// Keep the module registered (retry on startup + heartbeat to survive registry restarts)
builder.Services.AddHostedService<Sales.API.Services.ModuleRegistrationHostedService>();

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
        var context = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var seeder = new SalesSeeder(context, httpClientFactory);
        await seeder.SeedAsync();
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

// Dev/demo-only endpoint that clears and reseeds the in-memory Sales database so a
// fresh, fully-populated "tenant" dataset is available every time a user logs in with
// the admin/password demo credentials.
app.MapPost("/api/sales/tenant-reset", async (SalesDbContext context, IHttpClientFactory httpClientFactory) =>
{
    if (!useInMemory)
    {
        return Results.BadRequest("Tenant reset is only supported for the in-memory demo database.");
    }

    context.OrderPayments.RemoveRange(context.OrderPayments);
    context.OrderLineItems.RemoveRange(context.OrderLineItems);
    context.Orders.RemoveRange(context.Orders);
    context.QuoteLineItems.RemoveRange(context.QuoteLineItems);
    context.Quotes.RemoveRange(context.Quotes);
    await context.SaveChangesAsync();

    var seeder = new SalesSeeder(context, httpClientFactory);
    await seeder.SeedAsync();

    return Results.Ok(new { message = "Sales tenant database reset and reseeded." });
}).AllowAnonymousTenant();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
