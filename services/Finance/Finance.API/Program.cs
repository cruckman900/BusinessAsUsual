using Finance.Application.Services;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Finance.Application.Events;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Shared in-memory data store for the mock services (singleton so data persists)
builder.Services.AddSingleton<FinanceDataStore>();

// Register Finance services (using mock implementations for now)
builder.Services.AddScoped<IInvoiceService, MockInvoiceService>();
builder.Services.AddScoped<IPaymentService, MockPaymentService>();
builder.Services.AddScoped<IFinanceReportService, MockFinanceReportService>();

// Register HTTP client for module registration
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();

// In-process event bus + Finance consumers. When CRM (or a future broker
// bridge) publishes OpportunityWon into this process, Finance creates a draft
// invoice. See docs/FINANCE_MODULE_GUIDE.md for cross-service delivery.
builder.Services.AddInProcessEventBus();
builder.Services.AddIntegrationEventHandler<OpportunityWonIntegrationEvent, OpportunityWonHandler>();

// Keep the module registered (retry on startup + heartbeat to survive registry restarts)
builder.Services.AddHostedService<Finance.API.Services.ModuleRegistrationHostedService>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Exposed so WebApplicationFactory<Program> can bootstrap the API in functional tests.
public partial class Program { }
