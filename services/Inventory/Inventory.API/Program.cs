using Inventory.Application.Services;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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

// Register application services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<PurchaseOrderService>();
builder.Services.AddScoped<SupplierService>();

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
        SeedData(context);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static void SeedData(InventoryDbContext context)
{
    if (context.Products.Any()) return; // Already seeded

    var warehouse1Id = Guid.NewGuid();
    var warehouse2Id = Guid.NewGuid();

    var warehouse1 = new Inventory.Domain.Entities.Warehouse
    {
        Id = warehouse1Id,
        Name = "Main Warehouse",
        Code = "WH-001",
        Address = "123 Industrial Blvd",
        City = "Business City",
        State = "CA",
        ZipCode = "90210",
        Country = "USA",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var warehouse2 = new Inventory.Domain.Entities.Warehouse
    {
        Id = warehouse2Id,
        Name = "Secondary Warehouse",
        Code = "WH-002",
        Address = "456 Storage Ave",
        City = "Commerce Town",
        State = "CA",
        ZipCode = "90211",
        Country = "USA",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    context.Warehouses.AddRange(warehouse1, warehouse2);

    var supplier1Id = Guid.NewGuid();
    var supplier2Id = Guid.NewGuid();

    var supplier1 = new Inventory.Domain.Entities.Supplier
    {
        Id = supplier1Id,
        Name = "Acme Corp",
        Code = "SUP-001",
        ContactName = "John Doe",
        Email = "john@acme.com",
        Phone = "555-1234",
        Address = "789 Supply St",
        City = "Supplier City",
        State = "CA",
        ZipCode = "90212",
        Country = "USA",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var supplier2 = new Inventory.Domain.Entities.Supplier
    {
        Id = supplier2Id,
        Name = "Global Supplies Inc",
        Code = "SUP-002",
        ContactName = "Jane Smith",
        Email = "jane@globalsupplies.com",
        Phone = "555-5678",
        Address = "321 Trade Rd",
        City = "Vendor Town",
        State = "CA",
        ZipCode = "90213",
        Country = "USA",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    context.Suppliers.AddRange(supplier1, supplier2);

    var product1Id = Guid.NewGuid();
    var product2Id = Guid.NewGuid();
    var product3Id = Guid.NewGuid();
    var product4Id = Guid.NewGuid();

    var product1 = new Inventory.Domain.Entities.Product
    {
        Id = product1Id,
        Name = "Widget A",
        SKU = "WDG-A-001",
        Description = "High-quality widget",
        Cost = 19.99m,
        Price = 29.99m,
        ReorderPoint = 50,
        ReorderQuantity = 100,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var product2 = new Inventory.Domain.Entities.Product
    {
        Id = product2Id,
        Name = "Widget B",
        SKU = "WDG-B-002",
        Description = "Premium widget",
        Cost = 29.99m,
        Price = 49.99m,
        ReorderPoint = 30,
        ReorderQuantity = 75,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var product3 = new Inventory.Domain.Entities.Product
    {
        Id = product3Id,
        Name = "Gadget X",
        SKU = "GDG-X-003",
        Description = "Essential gadget",
        Cost = 9.99m,
        Price = 19.99m,
        ReorderPoint = 100,
        ReorderQuantity = 200,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var product4 = new Inventory.Domain.Entities.Product
    {
        Id = product4Id,
        Name = "Tool Z",
        SKU = "TL-Z-004",
        Description = "Professional tool",
        Cost = 49.99m,
        Price = 79.99m,
        ReorderPoint = 20,
        ReorderQuantity = 50,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    context.Products.AddRange(product1, product2, product3, product4);

    var stockItem1 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product1Id,
        WarehouseId = warehouse1Id,
        QuantityOnHand = 150,
        QuantityAllocated = 0,
        AverageCost = 19.99m,
        LastStockDate = DateTime.UtcNow.AddDays(-5),
        CreatedAt = DateTime.UtcNow
    };

    var stockItem2 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product2Id,
        WarehouseId = warehouse1Id,
        QuantityOnHand = 75,
        QuantityAllocated = 0,
        AverageCost = 29.99m,
        LastStockDate = DateTime.UtcNow.AddDays(-3),
        CreatedAt = DateTime.UtcNow
    };

    var stockItem3 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product3Id,
        WarehouseId = warehouse2Id,
        QuantityOnHand = 25, // Low stock!
        QuantityAllocated = 0,
        AverageCost = 9.99m,
        LastStockDate = DateTime.UtcNow.AddDays(-10),
        CreatedAt = DateTime.UtcNow
    };

    var stockItem4 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product4Id,
        WarehouseId = warehouse1Id,
        QuantityOnHand = 60,
        QuantityAllocated = 0,
        AverageCost = 49.99m,
        LastStockDate = DateTime.UtcNow.AddDays(-7),
        CreatedAt = DateTime.UtcNow
    };

    context.StockItems.AddRange(stockItem1, stockItem2, stockItem3, stockItem4);

    var po1 = new Inventory.Domain.Entities.PurchaseOrder
    {
        Id = Guid.NewGuid(),
        OrderNumber = "PO-2024-001",
        SupplierId = supplier1Id,
        WarehouseId = warehouse1Id,
        OrderDate = DateTime.UtcNow.AddDays(-2),
        ExpectedDeliveryDate = DateTime.UtcNow.AddDays(5),
        Status = Inventory.Domain.Entities.PurchaseOrderStatus.Submitted,
        SubTotal = 2999.00m,
        TaxAmount = 299.90m,
        ShippingCost = 50.00m,
        Total = 3348.90m,
        CreatedBy = "System",
        CreatedAt = DateTime.UtcNow
    };

    var po2 = new Inventory.Domain.Entities.PurchaseOrder
    {
        Id = Guid.NewGuid(),
        OrderNumber = "PO-2024-002",
        SupplierId = supplier2Id,
        WarehouseId = warehouse1Id,
        OrderDate = DateTime.UtcNow.AddDays(-1),
        ExpectedDeliveryDate = DateTime.UtcNow.AddDays(7),
        Status = Inventory.Domain.Entities.PurchaseOrderStatus.Submitted,
        SubTotal = 1499.00m,
        TaxAmount = 149.90m,
        ShippingCost = 25.00m,
        Total = 1673.90m,
        CreatedBy = "System",
        CreatedAt = DateTime.UtcNow
    };

    context.PurchaseOrders.AddRange(po1, po2);

    context.SaveChanges();
    Console.WriteLine("✅ Inventory database seeded with test data");
}
