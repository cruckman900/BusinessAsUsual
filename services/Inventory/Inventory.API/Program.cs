using Inventory.Application.Services;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;

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
    var product5Id = Guid.NewGuid();
    var product6Id = Guid.NewGuid();
    var product7Id = Guid.NewGuid();
    var product8Id = Guid.NewGuid();

    // Enterprise Software & Hardware
    var product1 = new Inventory.Domain.Entities.Product
    {
        Id = product1Id,
        Name = "Enterprise CRM Platform - Annual License",
        SKU = "SW-CRM-001",
        Description = "Full-featured customer relationship management platform with unlimited users",
        Category = "Software",
        Cost = 15000m,
        Price = 25000m,
        ReorderPoint = 0,
        ReorderQuantity = 0,
        IsTrackedInventory = false,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var product2 = new Inventory.Domain.Entities.Product
    {
        Id = product2Id,
        Name = "Professional Services - Implementation",
        SKU = "SVC-IMP-001",
        Description = "40 hours of professional implementation and training services",
        Category = "Services",
        UnitOfMeasure = "HR",
        Cost = 8000m,
        Price = 15000m,
        ReorderPoint = 0,
        ReorderQuantity = 0,
        IsTrackedInventory = false,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    // Medical Equipment
    var product3 = new Inventory.Domain.Entities.Product
    {
        Id = product3Id,
        Name = "Digital X-Ray Machine - Portable",
        SKU = "MED-XR-500",
        Description = "Portable digital X-ray imaging system with wireless connectivity",
        Category = "Medical Equipment",
        Cost = 45000m,
        Price = 75000m,
        ReorderPoint = 2,
        ReorderQuantity = 5,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    // Construction Materials
    var product4 = new Inventory.Domain.Entities.Product
    {
        Id = product4Id,
        Name = "Steel I-Beam - 20ft",
        SKU = "CONST-IB-20",
        Description = "Structural steel I-beam, 20 foot length, grade A36",
        Category = "Construction Materials",
        UnitOfMeasure = "EA",
        Cost = 450m,
        Price = 850m,
        ReorderPoint = 50,
        ReorderQuantity = 100,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var product5 = new Inventory.Domain.Entities.Product
    {
        Id = product5Id,
        Name = "Concrete - Ready Mix (per cubic yard)",
        SKU = "CONST-CM-001",
        Description = "Standard 3000 PSI ready-mix concrete",
        Category = "Construction Materials",
        UnitOfMeasure = "YD",
        Cost = 85m,
        Price = 145m,
        ReorderPoint = 0,
        ReorderQuantity = 0,
        IsTrackedInventory = false,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    // Office & School Supplies
    var product6 = new Inventory.Domain.Entities.Product
    {
        Id = product6Id,
        Name = "Interactive Smartboard - 75 inch",
        SKU = "EDU-SB-75",
        Description = "Interactive touchscreen display for classroom or conference room",
        Category = "Education Technology",
        Cost = 2500m,
        Price = 4500m,
        ReorderPoint = 10,
        ReorderQuantity = 25,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    var product7 = new Inventory.Domain.Entities.Product
    {
        Id = product7Id,
        Name = "Student Desk & Chair Set",
        SKU = "EDU-DSK-001",
        Description = "Adjustable student desk with ergonomic chair",
        Category = "Education Furniture",
        Cost = 120m,
        Price = 250m,
        ReorderPoint = 50,
        ReorderQuantity = 100,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    // Retail Tech
    var product8 = new Inventory.Domain.Entities.Product
    {
        Id = product8Id,
        Name = "Point of Sale Terminal",
        SKU = "RET-POS-001",
        Description = "All-in-one touchscreen POS terminal with receipt printer and card reader",
        Category = "Retail Hardware",
        Cost = 650m,
        Price = 1200m,
        ReorderPoint = 20,
        ReorderQuantity = 40,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    context.Products.AddRange(product1, product2, product3, product4, product5, product6, product7, product8);

    var stockItem1 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product3Id, // X-Ray Machine
        WarehouseId = warehouse1Id,
        QuantityOnHand = 5,
        QuantityAllocated = 1,
        AverageCost = 45000m,
        LastStockDate = DateTime.UtcNow.AddDays(-5),
        CreatedAt = DateTime.UtcNow
    };

    var stockItem2 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product4Id, // I-Beams
        WarehouseId = warehouse1Id,
        QuantityOnHand = 150,
        QuantityAllocated = 50,
        AverageCost = 450m,
        LastStockDate = DateTime.UtcNow.AddDays(-3),
        CreatedAt = DateTime.UtcNow
    };

    var stockItem3 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product6Id, // Smartboards
        WarehouseId = warehouse1Id,
        QuantityOnHand = 25,
        QuantityAllocated = 5,
        AverageCost = 2500m,
        LastStockDate = DateTime.UtcNow.AddDays(-2),
        CreatedAt = DateTime.UtcNow
    };

    var stockItem4 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product7Id, // Desk sets
        WarehouseId = warehouse1Id,
        QuantityOnHand = 200,
        QuantityAllocated = 50,
        AverageCost = 120m,
        LastStockDate = DateTime.UtcNow.AddDays(-1),
        CreatedAt = DateTime.UtcNow
    };

    var stockItem5 = new Inventory.Domain.Entities.StockItem
    {
        Id = Guid.NewGuid(),
        ProductId = product8Id, // POS terminals
        WarehouseId = warehouse2Id,
        QuantityOnHand = 45,
        QuantityAllocated = 10,
        AverageCost = 650m,
        LastStockDate = DateTime.UtcNow.AddDays(-1),
        CreatedAt = DateTime.UtcNow
    };

    context.StockItems.AddRange(stockItem1, stockItem2, stockItem3, stockItem4, stockItem5);

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

// Make Program accessible for integration tests
public partial class Program { }
