using Sales.Application.Services;
using Sales.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

// Register application services
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IOrderService, OrderService>();

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

static void SeedData(SalesDbContext context)
{
    if (context.Quotes.Any()) return; // Already seeded

    var quote1 = new Sales.Domain.Entities.Quote
    {
        Id = Guid.NewGuid().ToString(),
        QuoteNumber = "Q-20260801-0001",
        CustomerId = "CUST001",
        CustomerName = "Acme Corporation",
        CustomerEmail = "purchasing@acme.com",
        CustomerPhone = "+1-555-0100",
        Status = Sales.Domain.Enums.QuoteStatus.Sent,
        Currency = Sales.Domain.Enums.Currency.USD,
        CreatedDate = DateTime.UtcNow.AddDays(-7),
        SentDate = DateTime.UtcNow.AddDays(-6),
        ExpiryDate = DateTime.UtcNow.AddDays(23),
        Notes = "Enterprise licensing quote for Q3 2026",
        Terms = "Net 30",
        AssignedToEmployeeId = "EMP123"
    };

    quote1.LineItems = new List<Sales.Domain.Entities.QuoteLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = quote1.Id,
            ProductId = "PROD001",
            ProductName = "Enterprise Software License",
            Description = "Annual subscription for 100 users",
            Quantity = 100,
            UnitPrice = 149.99m,
            DiscountPercentage = 10,
            TaxPercentage = 8.5m,
            SortOrder = 0
        },
        new()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = quote1.Id,
            ProductId = "PROD002",
            ProductName = "Premium Support",
            Description = "24/7 phone and email support",
            Quantity = 1,
            UnitPrice = 2999.99m,
            DiscountPercentage = 0,
            TaxPercentage = 8.5m,
            SortOrder = 1
        }
    };

    var order1 = new Sales.Domain.Entities.Order
    {
        Id = Guid.NewGuid().ToString(),
        OrderNumber = "O-20260801-0001",
        CustomerId = "CUST002",
        CustomerName = "TechStart Inc",
        CustomerEmail = "orders@techstart.com",
        CustomerPhone = "+1-555-0200",
        Status = Sales.Domain.Enums.OrderStatus.Confirmed,
        Currency = Sales.Domain.Enums.Currency.USD,
        OrderDate = DateTime.UtcNow.AddDays(-3),
        ConfirmedDate = DateTime.UtcNow.AddDays(-2),
        ShippingMethod = Sales.Domain.Enums.ShippingMethod.Express,
        ShippingAddressLine1 = "456 Tech Blvd",
        ShippingCity = "San Francisco",
        ShippingState = "CA",
        ShippingPostalCode = "94105",
        ShippingCountry = "USA",
        BillingAddressLine1 = "456 Tech Blvd",
        BillingCity = "San Francisco",
        BillingState = "CA",
        BillingPostalCode = "94105",
        BillingCountry = "USA",
        Notes = "Expedite shipping requested",
        AssignedToEmployeeId = "EMP456",
        ShippingCost = 45.00m
    };

    order1.LineItems = new List<Sales.Domain.Entities.OrderLineItem>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order1.Id,
            ProductId = "PROD003",
            ProductName = "Developer Workstation",
            SKU = "DEV-WS-001",
            Description = "High-performance development laptop",
            Quantity = 5,
            UnitPrice = 2499.99m,
            DiscountPercentage = 5,
            TaxPercentage = 8.5m,
            SortOrder = 0
        }
    };

    order1.Payments = new List<Sales.Domain.Entities.OrderPayment>
    {
        new()
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = order1.Id,
            PaymentMethod = Sales.Domain.Enums.PaymentMethod.CreditCard,
            Amount = 6000.00m,
            PaymentDate = DateTime.UtcNow.AddDays(-2),
            TransactionId = "TXN-20260729-001",
            ReferenceNumber = "AUTH-5678",
            IsCompleted = true
        }
    };

    context.Quotes.Add(quote1);
    context.Orders.Add(order1);
    context.SaveChanges();

    Console.WriteLine("✅ Sales database seeded with test data");
}
