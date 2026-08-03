using Sales.Application.Services;
using Sales.Infrastructure.Persistence;
using Sales.Infrastructure.Repositories;
using Sales.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Core.Events;

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

// Register application services
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Register event bus for cross-module integration events
builder.Services.AddInProcessEventBus();

// Register HTTP clients for cross-module communication
builder.Services.AddHttpClient("InventoryApi", client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("InventoryApi:Url") ?? "http://localhost:5142";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("CrmApi", client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("CrmApi:Url") ?? "http://localhost:5004";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

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
        await SeedDataAsync(context, httpClientFactory);
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

static async Task SeedDataAsync(SalesDbContext context, IHttpClientFactory httpClientFactory)
{
    if (context.Quotes.Any()) return; // Already seeded

    Console.WriteLine("🔄 Fetching products from Inventory API...");
    var inventoryClient = httpClientFactory.CreateClient("InventoryApi");
    List<ProductDto>? products = null;
    try
    {
        products = await inventoryClient.GetFromJsonAsync<List<ProductDto>>("api/inventory/products");
        Console.WriteLine($"✅ Fetched {products?.Count ?? 0} products from Inventory");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Could not fetch products from Inventory API: {ex.Message}");
        Console.WriteLine("   Continuing with hardcoded product IDs...");
    }

    Console.WriteLine("🔄 Fetching customers from CRM API...");
    var crmClient = httpClientFactory.CreateClient("CrmApi");
    List<CustomerDto>? customers = null;
    try
    {
        customers = await crmClient.GetFromJsonAsync<List<CustomerDto>>("api/crm/customers");
        Console.WriteLine($"✅ Fetched {customers?.Count ?? 0} customers from CRM");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Could not fetch customers from CRM API: {ex.Message}");
        Console.WriteLine("   Continuing with hardcoded customer data...");
    }

    // Helper to get product or fallback
    ProductDto? GetProduct(string sku) => products?.FirstOrDefault(p => p.SKU == sku);
    CustomerDto? GetCustomer(string id) => customers?.FirstOrDefault(c => c.Id == id);

    // Get specific products by SKU
    var crmProduct = GetProduct("SW-CRM-001");
    var servicesProduct = GetProduct("SVC-IMP-001");
    var xrayProduct = GetProduct("MED-XR-500");
    var smartboardProduct = GetProduct("EDU-SB-75");
    var deskProduct = GetProduct("EDU-DSK-001");
    var ibeamProduct = GetProduct("CONST-IB-20");
    var concreteProduct = GetProduct("CONST-CM-001");
    var posProduct = GetProduct("RET-POS-001");

    // Get specific customers by ID
    var techCorpCustomer = GetCustomer("C1");
    var cityMedicalCustomer = GetCustomer("C3");
    var oakwoodSchoolCustomer = GetCustomer("C4");
    var buildRightCustomer = GetCustomer("C5");
    var retailPlusCustomer = GetCustomer("C6");

    // === QUOTE 1: Enterprise Software Deal (TechCorp) ===
    var quote1 = new Sales.Domain.Entities.Quote
    {
        Id = Guid.NewGuid().ToString(),
        QuoteNumber = "Q-2025-0001",
        CustomerId = techCorpCustomer?.Id ?? "C1",
        CustomerName = techCorpCustomer?.Name ?? "TechCorp International",
        CustomerEmail = techCorpCustomer?.Email ?? "contact@techcorp.com",
        CustomerPhone = techCorpCustomer?.Phone ?? "555-1001",
        Status = Sales.Domain.Enums.QuoteStatus.Sent,
        Currency = Sales.Domain.Enums.Currency.USD,
        CreatedDate = DateTime.UtcNow.AddDays(-7),
        SentDate = DateTime.UtcNow.AddDays(-6),
        ExpiryDate = DateTime.UtcNow.AddDays(23),
        Notes = "Enterprise CRM platform licensing with professional services",
        Terms = "Net 30",
        AssignedToEmployeeId = "EMP123"
    };
    quote1.LineItems = new List<Sales.Domain.Entities.QuoteLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = quote1.Id,
            ProductId = crmProduct?.Id.ToString() ?? "PROD-CRM",
            ProductName = crmProduct?.Name ?? "Enterprise CRM Platform - Annual License",
            Description = crmProduct?.Description ?? "Full-featured CRM platform",
            Quantity = 1,
            UnitPrice = crmProduct?.Price ?? 25000m,
            DiscountPercentage = 10,
            TaxPercentage = 8.5m,
            SortOrder = 0
        },
        new()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = quote1.Id,
            ProductId = servicesProduct?.Id.ToString() ?? "PROD-SVC",
            ProductName = servicesProduct?.Name ?? "Professional Services - Implementation",
            Description = servicesProduct?.Description ?? "40 hours of implementation services",
            Quantity = 1,
            UnitPrice = servicesProduct?.Price ?? 15000m,
            DiscountPercentage = 0,
            TaxPercentage = 8.5m,
            SortOrder = 1
        }
    };

    // === QUOTE 2: Medical Equipment (City Medical Center) ===
    var quote2 = new Sales.Domain.Entities.Quote
    {
        Id = Guid.NewGuid().ToString(),
        QuoteNumber = "Q-2025-0002",
        CustomerId = cityMedicalCustomer?.Id ?? "C3",
        CustomerName = cityMedicalCustomer?.Name ?? "City Medical Center",
        CustomerEmail = cityMedicalCustomer?.Email ?? "procurement@citymedical.org",
        CustomerPhone = cityMedicalCustomer?.Phone ?? "555-2000",
        Status = Sales.Domain.Enums.QuoteStatus.Accepted,
        Currency = Sales.Domain.Enums.Currency.USD,
        CreatedDate = DateTime.UtcNow.AddDays(-10),
        SentDate = DateTime.UtcNow.AddDays(-9),
        AcceptedDate = DateTime.UtcNow.AddDays(-1),
        ExpiryDate = DateTime.UtcNow.AddDays(20),
        Notes = "Digital X-ray equipment for radiology department expansion",
        Terms = "Net 30",
        AssignedToEmployeeId = "EMP123"
    };
    quote2.LineItems = new List<Sales.Domain.Entities.QuoteLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = quote2.Id,
            ProductId = xrayProduct?.Id.ToString() ?? "PROD-XRAY",
            ProductName = xrayProduct?.Name ?? "Digital X-Ray Machine - Portable",
            Description = xrayProduct?.Description ?? "Portable digital X-ray system",
            Quantity = 1,
            UnitPrice = xrayProduct?.Price ?? 75000m,
            DiscountPercentage = 5,
            TaxPercentage = 0,
            SortOrder = 0
        }
    };

    // === QUOTE 3: Education Technology (Oakwood Schools) ===
    var quote3 = new Sales.Domain.Entities.Quote
    {
        Id = Guid.NewGuid().ToString(),
        QuoteNumber = "Q-2025-0003",
        CustomerId = oakwoodSchoolCustomer?.Id ?? "C4",
        CustomerName = oakwoodSchoolCustomer?.Name ?? "Oakwood School District",
        CustomerEmail = oakwoodSchoolCustomer?.Email ?? "purchasing@oakwoodschools.edu",
        CustomerPhone = oakwoodSchoolCustomer?.Phone ?? "555-3000",
        Status = Sales.Domain.Enums.QuoteStatus.Draft,
        Currency = Sales.Domain.Enums.Currency.USD,
        CreatedDate = DateTime.UtcNow.AddDays(-2),
        ExpiryDate = DateTime.UtcNow.AddDays(28),
        Notes = "Classroom technology upgrade for 10 classrooms",
        Terms = "Net 45 - School District payment terms",
        AssignedToEmployeeId = "EMP124"
    };
    quote3.LineItems = new List<Sales.Domain.Entities.QuoteLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = quote3.Id,
            ProductId = smartboardProduct?.Id.ToString() ?? "PROD-SMART",
            ProductName = smartboardProduct?.Name ?? "Interactive Smartboard - 75 inch",
            Description = smartboardProduct?.Description ?? "Interactive touchscreen display",
            Quantity = 10,
            UnitPrice = smartboardProduct?.Price ?? 4500m,
            DiscountPercentage = 15,
            TaxPercentage = 6.25m,
            SortOrder = 0
        },
        new()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = quote3.Id,
            ProductId = deskProduct?.Id.ToString() ?? "PROD-DESK",
            ProductName = deskProduct?.Name ?? "Student Desk & Chair Set",
            Description = deskProduct?.Description ?? "Adjustable student desk with chair",
            Quantity = 50,
            UnitPrice = deskProduct?.Price ?? 250m,
            DiscountPercentage = 20,
            TaxPercentage = 6.25m,
            SortOrder = 1
        }
    };

    // === ORDER 1: Construction Materials (BuildRight) - CONFIRMED ===
    var order1 = new Sales.Domain.Entities.Order
    {
        Id = Guid.NewGuid().ToString(),
        OrderNumber = "O-2025-0001",
        CustomerId = buildRightCustomer?.Id ?? "C5",
        CustomerName = buildRightCustomer?.Name ?? "BuildRight Construction",
        CustomerEmail = buildRightCustomer?.Email ?? "materials@buildright.com",
        CustomerPhone = buildRightCustomer?.Phone ?? "555-4000",
        Status = Sales.Domain.Enums.OrderStatus.Confirmed,
        Currency = Sales.Domain.Enums.Currency.USD,
        OrderDate = DateTime.UtcNow.AddDays(-5),
        ConfirmedDate = DateTime.UtcNow.AddDays(-4),
        ShippingMethod = Sales.Domain.Enums.ShippingMethod.Standard,
        ShippingAddressLine1 = buildRightCustomer?.ShippingAddressLine1 ?? "2500 Construction Site Rd",
        ShippingAddressLine2 = buildRightCustomer?.ShippingAddressLine2 ?? "Site Office #3",
        ShippingCity = buildRightCustomer?.ShippingCity ?? "Denver",
        ShippingState = buildRightCustomer?.ShippingState ?? "CO",
        ShippingPostalCode = buildRightCustomer?.ShippingPostalCode ?? "80203",
        ShippingCountry = buildRightCustomer?.ShippingCountry ?? "USA",
        BillingAddressLine1 = buildRightCustomer?.BillingAddressLine1 ?? "800 Builder's Row",
        BillingCity = buildRightCustomer?.BillingCity ?? "Denver",
        BillingState = buildRightCustomer?.BillingState ?? "CO",
        BillingPostalCode = buildRightCustomer?.BillingPostalCode ?? "80202",
        BillingCountry = buildRightCustomer?.BillingCountry ?? "USA",
        Notes = "High-rise construction project materials",
        AssignedToEmployeeId = "EMP125",
        ShippingCost = 350.00m
    };
    order1.LineItems = new List<Sales.Domain.Entities.OrderLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order1.Id,
            ProductId = ibeamProduct?.Id.ToString() ?? "PROD-IBEAM",
            ProductName = ibeamProduct?.Name ?? "Steel I-Beam - 20ft",
            SKU = ibeamProduct?.SKU ?? "CONST-IB-20",
            Description = ibeamProduct?.Description ?? "Structural steel I-beam",
            Quantity = 50,
            UnitPrice = ibeamProduct?.Price ?? 850m,
            DiscountPercentage = 10,
            TaxPercentage = 8.25m,
            SortOrder = 0
        },
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order1.Id,
            ProductId = concreteProduct?.Id.ToString() ?? "PROD-CONCRETE",
            ProductName = concreteProduct?.Name ?? "Concrete - Ready Mix (per cubic yard)",
            SKU = concreteProduct?.SKU ?? "CONST-CM-001",
            Description = concreteProduct?.Description ?? "Standard 3000 PSI ready-mix concrete",
            Quantity = 100,
            UnitPrice = concreteProduct?.Price ?? 145m,
            DiscountPercentage = 5,
            TaxPercentage = 8.25m,
            SortOrder = 1
        }
    };
    order1.Payments = new List<Sales.Domain.Entities.OrderPayment>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order1.Id,
            PaymentMethod = Sales.Domain.Enums.PaymentMethod.Check,
            Amount = 30000.00m,
            PaymentDate = DateTime.UtcNow.AddDays(-4),
            TransactionId = "CHK-2025-BR-001",
            ReferenceNumber = "CHK-BR-456",
            IsCompleted = true
        }
    };

    // === ORDER 2: Retail POS Systems (Retail Plus) - SHIPPED ===
    var order2 = new Sales.Domain.Entities.Order
    {
        Id = Guid.NewGuid().ToString(),
        OrderNumber = "O-2025-0002",
        CustomerId = retailPlusCustomer?.Id ?? "C6",
        CustomerName = retailPlusCustomer?.Name ?? "Retail Plus Stores",
        CustomerEmail = retailPlusCustomer?.Email ?? "it@retailplus.com",
        CustomerPhone = retailPlusCustomer?.Phone ?? "555-5000",
        Status = Sales.Domain.Enums.OrderStatus.Shipped,
        Currency = Sales.Domain.Enums.Currency.USD,
        OrderDate = DateTime.UtcNow.AddDays(-15),
        ConfirmedDate = DateTime.UtcNow.AddDays(-14),
        ShippedDate = DateTime.UtcNow.AddDays(-10),
        ShippingMethod = Sales.Domain.Enums.ShippingMethod.Express,
        TrackingNumber = "1Z999AA10123456784",
        ShippingAddressLine1 = retailPlusCustomer?.ShippingAddressLine1 ?? "300 Retail Plaza, IT Department",
        ShippingCity = retailPlusCustomer?.ShippingCity ?? "Dallas",
        ShippingState = retailPlusCustomer?.ShippingState ?? "TX",
        ShippingPostalCode = retailPlusCustomer?.ShippingPostalCode ?? "75201",
        ShippingCountry = retailPlusCustomer?.ShippingCountry ?? "USA",
        BillingAddressLine1 = retailPlusCustomer?.BillingAddressLine1 ?? "300 Retail Plaza",
        BillingCity = retailPlusCustomer?.BillingCity ?? "Dallas",
        BillingState = retailPlusCustomer?.BillingState ?? "TX",
        BillingPostalCode = retailPlusCustomer?.BillingPostalCode ?? "75201",
        BillingCountry = retailPlusCustomer?.BillingCountry ?? "USA",
        Notes = "Store technology refresh - 15 locations",
        AssignedToEmployeeId = "EMP456",
        ShippingCost = 175.00m
    };
    order2.LineItems = new List<Sales.Domain.Entities.OrderLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order2.Id,
            ProductId = posProduct?.Id.ToString() ?? "PROD-POS",
            ProductName = posProduct?.Name ?? "Point of Sale Terminal",
            SKU = posProduct?.SKU ?? "RET-POS-001",
            Description = posProduct?.Description ?? "All-in-one touchscreen POS terminal",
            Quantity = 45,
            UnitPrice = posProduct?.Price ?? 1200m,
            DiscountPercentage = 15,
            TaxPercentage = 8.25m,
            SortOrder = 0
        }
    };
    order2.Payments = new List<Sales.Domain.Entities.OrderPayment>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order2.Id,
            PaymentMethod = Sales.Domain.Enums.PaymentMethod.CreditCard,
            Amount = 50000.00m,
            PaymentDate = DateTime.UtcNow.AddDays(-14),
            TransactionId = "TXN-2025-RP-001",
            ReferenceNumber = "AUTH-5678",
            IsCompleted = true
        }
    };

    // === ORDER 3: Education Equipment (Oakwood Schools) - DELIVERED ===
    var order3 = new Sales.Domain.Entities.Order
    {
        Id = Guid.NewGuid().ToString(),
        OrderNumber = "O-2025-0003",
        CustomerId = oakwoodSchoolCustomer?.Id ?? "C4",
        CustomerName = oakwoodSchoolCustomer?.Name ?? "Oakwood School District",
        CustomerEmail = oakwoodSchoolCustomer?.Email ?? "purchasing@oakwoodschools.edu",
        CustomerPhone = oakwoodSchoolCustomer?.Phone ?? "555-3000",
        Status = Sales.Domain.Enums.OrderStatus.Delivered,
        Currency = Sales.Domain.Enums.Currency.USD,
        OrderDate = DateTime.UtcNow.AddDays(-30),
        ConfirmedDate = DateTime.UtcNow.AddDays(-29),
        ShippedDate = DateTime.UtcNow.AddDays(-25),
        DeliveredDate = DateTime.UtcNow.AddDays(-22),
        ShippingMethod = Sales.Domain.Enums.ShippingMethod.Standard,
        TrackingNumber = "1Z999AA10123456799",
        ShippingAddressLine1 = oakwoodSchoolCustomer?.ShippingAddressLine1 ?? "450 Education Lane, District Office",
        ShippingCity = oakwoodSchoolCustomer?.ShippingCity ?? "Portland",
        ShippingState = oakwoodSchoolCustomer?.ShippingState ?? "OR",
        ShippingPostalCode = oakwoodSchoolCustomer?.ShippingPostalCode ?? "97201",
        ShippingCountry = oakwoodSchoolCustomer?.ShippingCountry ?? "USA",
        BillingAddressLine1 = oakwoodSchoolCustomer?.BillingAddressLine1 ?? "450 Education Lane",
        BillingCity = oakwoodSchoolCustomer?.BillingCity ?? "Portland",
        BillingState = oakwoodSchoolCustomer?.BillingState ?? "OR",
        BillingPostalCode = oakwoodSchoolCustomer?.BillingPostalCode ?? "97201",
        BillingCountry = oakwoodSchoolCustomer?.BillingCountry ?? "USA",
        Notes = "Previous semester furniture order",
        AssignedToEmployeeId = "EMP126",
        ShippingCost = 225.00m
    };
    order3.LineItems = new List<Sales.Domain.Entities.OrderLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order3.Id,
            ProductId = deskProduct?.Id.ToString() ?? "PROD-DESK",
            ProductName = deskProduct?.Name ?? "Student Desk & Chair Set",
            SKU = deskProduct?.SKU ?? "EDU-DSK-001",
            Description = deskProduct?.Description ?? "Adjustable student desk with chair",
            Quantity = 150,
            UnitPrice = deskProduct?.Price ?? 250m,
            DiscountPercentage = 20,
            TaxPercentage = 6.25m,
            SortOrder = 0
        }
    };
    order3.Payments = new List<Sales.Domain.Entities.OrderPayment>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order3.Id,
            PaymentMethod = Sales.Domain.Enums.PaymentMethod.BankTransfer,
            Amount = 32000.00m,
            PaymentDate = DateTime.UtcNow.AddDays(-28),
            TransactionId = "WIRE-2025-OAK-001",
            ReferenceNumber = "WIRE-OAK-789",
            IsCompleted = true
        }
    };

    // === ORDER 4: Medical Equipment (City Medical) - PROCESSING ===
    var order4 = new Sales.Domain.Entities.Order
    {
        Id = Guid.NewGuid().ToString(),
        OrderNumber = "O-2025-0004",
        CustomerId = cityMedicalCustomer?.Id ?? "C3",
        CustomerName = cityMedicalCustomer?.Name ?? "City Medical Center",
        CustomerEmail = cityMedicalCustomer?.Email ?? "procurement@citymedical.org",
        CustomerPhone = cityMedicalCustomer?.Phone ?? "555-2000",
        Status = Sales.Domain.Enums.OrderStatus.Processing,
        Currency = Sales.Domain.Enums.Currency.USD,
        OrderDate = DateTime.UtcNow.AddDays(-2),
        ConfirmedDate = DateTime.UtcNow.AddDays(-1),
        ShippingMethod = Sales.Domain.Enums.ShippingMethod.Overnight,
        ShippingAddressLine1 = cityMedicalCustomer?.ShippingAddressLine1 ?? "100 Hospital Drive, Receiving Dock B",
        ShippingCity = cityMedicalCustomer?.ShippingCity ?? "Chicago",
        ShippingState = cityMedicalCustomer?.ShippingState ?? "IL",
        ShippingPostalCode = cityMedicalCustomer?.ShippingPostalCode ?? "60601",
        ShippingCountry = cityMedicalCustomer?.ShippingCountry ?? "USA",
        BillingAddressLine1 = cityMedicalCustomer?.BillingAddressLine1 ?? "100 Hospital Drive",
        BillingCity = cityMedicalCustomer?.BillingCity ?? "Chicago",
        BillingState = cityMedicalCustomer?.BillingState ?? "IL",
        BillingPostalCode = cityMedicalCustomer?.BillingPostalCode ?? "60601",
        BillingCountry = cityMedicalCustomer?.BillingCountry ?? "USA",
        Notes = "X-Ray equipment installation - converted from Quote Q-2025-0002",
        AssignedToEmployeeId = "EMP123",
        ShippingCost = 500.00m
    };
    order4.LineItems = new List<Sales.Domain.Entities.OrderLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order4.Id,
            ProductId = xrayProduct?.Id.ToString() ?? "PROD-XRAY",
            ProductName = xrayProduct?.Name ?? "Digital X-Ray Machine - Portable",
            SKU = xrayProduct?.SKU ?? "MED-XR-500",
            Description = xrayProduct?.Description ?? "Portable digital X-ray system",
            Quantity = 1,
            UnitPrice = xrayProduct?.Price ?? 75000m,
            DiscountPercentage = 5,
            TaxPercentage = 0,
            SortOrder = 0
        }
    };
    order4.Payments = new List<Sales.Domain.Entities.OrderPayment>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order4.Id,
            PaymentMethod = Sales.Domain.Enums.PaymentMethod.BankTransfer,
            Amount = 71250.00m,
            PaymentDate = DateTime.UtcNow.AddDays(-1),
            TransactionId = "WIRE-2025-CMC-001",
            ReferenceNumber = "WIRE-CMC-123",
            IsCompleted = true
        }
    };

    context.Quotes.AddRange(quote1, quote2, quote3);
    context.Orders.AddRange(order1, order2, order3, order4);
    context.SaveChanges();

    Console.WriteLine("✅ Sales database seeded with integrated cross-module data:");
    Console.WriteLine($"   • {context.Quotes.Count()} Quotes (with real Inventory products & CRM customers)");
    Console.WriteLine($"   • {context.Orders.Count()} Orders (with real Inventory products & CRM customers)");
    Console.WriteLine($"   • {context.Orders.Sum(o => o.LineItems.Count)} Order Line Items");
    Console.WriteLine($"   • {context.Orders.Sum(o => o.Payments.Count)} Payments");
}

// DTOs for API responses
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CustomerDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }
    public string? ShippingAddressLine1 { get; set; }
    public string? ShippingAddressLine2 { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingCountry { get; set; }
}

// Make Program accessible for integration tests
public partial class Program { }
