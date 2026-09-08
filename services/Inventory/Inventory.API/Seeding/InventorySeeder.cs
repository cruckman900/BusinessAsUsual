using BusinessAsUsual.Infrastructure.SeedData;
using Inventory.Infrastructure.Persistence;

namespace Inventory.API.Seeding;

/// <summary>
/// Dev/demo-only in-memory seed data for the Inventory module. Deliberately kept out of
/// Program.cs so the host bootstrap file stays small and readable; this class is the
/// single place to look when the demo dataset needs to change.
/// Follows the same instantiable-seeder shape as Services.Infrastructure.Seeding.DataSeeder
/// and Sales.API.Seeding.SalesSeeder so every module's Program.cs wires up seeding the same
/// way: `new {Module}Seeder(...).SeedAsync()`.
/// </summary>
public class InventorySeeder : IModuleSeeder<InventoryDbContext>
{
    private readonly InventoryDbContext _context;

    public InventorySeeder(InventoryDbContext context)
    {
        _context = context;
    }

    // Explicit IModuleSeeder<InventoryDbContext> implementation: this seeder is constructed
    // with its DbContext up front, so the interface's context parameter is only honored here
    // for contract compliance/testability; the constructor-provided context remains the
    // source of truth. Callers should normally use the parameterless SeedAsync() below.
    Task IModuleSeeder<InventoryDbContext>.SeedAsync(InventoryDbContext context) => SeedAsync();

    public Task SeedAsync()
    {
        var context = _context;
        if (context.Products.Any()) return Task.CompletedTask; // Already seeded

        var demoCompanyId = DemoTenant.CompanyId;

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

        warehouse1.CompanyId = demoCompanyId;
        warehouse2.CompanyId = demoCompanyId;
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

        supplier1.CompanyId = demoCompanyId;
        supplier2.CompanyId = demoCompanyId;
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
            UnitOfMeasure = "EA",
            Cost = 2500m,
            Price = 4500m,
            ReorderPoint = 10,
            ReorderQuantity = 25,
            IsTrackedInventory = true,
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
            UnitOfMeasure = "EA",
            Cost = 120m,
            Price = 250m,
            ReorderPoint = 50,
            ReorderQuantity = 100,
            IsTrackedInventory = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Retail Hardware
        var product8 = new Inventory.Domain.Entities.Product
        {
            Id = product8Id,
            Name = "Point of Sale Terminal",
            SKU = "RET-POS-001",
            Description = "All-in-one touchscreen POS terminal with receipt printer and card reader",
            Category = "Retail Hardware",
            UnitOfMeasure = "EA",
            Cost = 650m,
            Price = 1200m,
            ReorderPoint = 20,
            ReorderQuantity = 40,
            IsTrackedInventory = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        product1.CompanyId = demoCompanyId;
        product2.CompanyId = demoCompanyId;
        product3.CompanyId = demoCompanyId;
        product4.CompanyId = demoCompanyId;
        product5.CompanyId = demoCompanyId;
        product6.CompanyId = demoCompanyId;
        product7.CompanyId = demoCompanyId;
        product8.CompanyId = demoCompanyId;
        context.Products.AddRange(product1, product2, product3, product4, product5, product6, product7, product8);

        // Stock levels for tracked-inventory products (untracked products: CRM license,
        // implementation services, ready-mix concrete have no stock items).
        var stockItems = new List<Inventory.Domain.Entities.StockItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = demoCompanyId,
                ProductId = product3Id,
                WarehouseId = warehouse1Id,
                QuantityOnHand = 5,
                QuantityAllocated = 1,
                AverageCost = product3.Cost,
                LastStockDate = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = demoCompanyId,
                ProductId = product4Id,
                WarehouseId = warehouse1Id,
                QuantityOnHand = 150,
                QuantityAllocated = 50,
                AverageCost = product4.Cost,
                LastStockDate = DateTime.UtcNow.AddDays(-3),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = demoCompanyId,
                ProductId = product6Id,
                WarehouseId = warehouse1Id,
                QuantityOnHand = 25,
                QuantityAllocated = 5,
                AverageCost = product6.Cost,
                LastStockDate = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = demoCompanyId,
                ProductId = product7Id,
                WarehouseId = warehouse1Id,
                QuantityOnHand = 200,
                QuantityAllocated = 50,
                AverageCost = product7.Cost,
                LastStockDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = demoCompanyId,
                ProductId = product8Id,
                WarehouseId = warehouse2Id,
                QuantityOnHand = 45,
                QuantityAllocated = 10,
                AverageCost = product8.Cost,
                LastStockDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow
            }
        };
        context.StockItems.AddRange(stockItems);

        // Open purchase orders (no line items in the demo dataset).
        var purchaseOrders = new List<Inventory.Domain.Entities.PurchaseOrder>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = demoCompanyId,
                OrderNumber = "PO-2024-001",
                SupplierId = supplier1Id,
                WarehouseId = warehouse1Id,
                Status = Inventory.Domain.Entities.PurchaseOrderStatus.Submitted,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                ExpectedDeliveryDate = DateTime.UtcNow.AddDays(5),
                SubTotal = 2999.00m,
                ShippingCost = 50.00m,
                TaxAmount = 299.90m,
                Total = 3348.90m,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = demoCompanyId,
                OrderNumber = "PO-2024-002",
                SupplierId = supplier2Id,
                WarehouseId = warehouse1Id,
                Status = Inventory.Domain.Entities.PurchaseOrderStatus.Submitted,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                ExpectedDeliveryDate = DateTime.UtcNow.AddDays(7),
                SubTotal = 1499.00m,
                ShippingCost = 25.00m,
                TaxAmount = 149.90m,
                Total = 1673.90m,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            }
        };
        context.PurchaseOrders.AddRange(purchaseOrders);

        context.SaveChanges();

        Console.WriteLine("✅ Inventory database seeded with demo data:");
        Console.WriteLine($"   • {context.Warehouses.Count()} Warehouses");
        Console.WriteLine($"   • {context.Suppliers.Count()} Suppliers");
        Console.WriteLine($"   • {context.Products.Count()} Products");
        Console.WriteLine($"   • {context.StockItems.Count()} Stock Items");
        Console.WriteLine($"   • {context.PurchaseOrders.Count()} Purchase Orders");

        return Task.CompletedTask;
    }
}
