using Inventory.Web.Components;
using Inventory.Application.Services;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - using Blazor Server to match shell
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor services
builder.Services.AddMudServices();

// Database configuration - use in-memory for development
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
    Console.WriteLine("⚠️  Inventory.Web using in-memory database");
    builder.Services.AddDbContext<InventoryDbContext>(options =>
        options.UseInMemoryDatabase("Inventory_Web"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("InventoryDatabase") 
        ?? "Server=localhost;Database=BusinessAsUsual_Inventory;Trusted_Connection=True;TrustServerCertificate=True;";
    builder.Services.AddDbContext<InventoryDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IStockItemRepository, StockItemRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();

// Register services
builder.Services.AddScoped<ProductService>();

// Named HTTP client for the Inventory API
var inventoryApiUrl = builder.Configuration["InventoryService:Url"] ?? "http://localhost:5142";
builder.Services.AddHttpClient("InventoryApi", client =>
{
    client.BaseAddress = new Uri(inventoryApiUrl);
});

// CORS configuration for iframe embedding
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebShell", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5000",  // Main web shell
            "https://localhost:7000"  // Main web shell HTTPS
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowWebShell");
app.UseAntiforgery();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
