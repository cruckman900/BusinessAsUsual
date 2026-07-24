using BusinessAsUsual.Core.Events;
using HR.Application.Services;
using HR.Domain.Repositories;
using HR.Infrastructure;
using HR.Infrastructure.Persistence;
using HR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

// Load environment variables from .env file
await ConfigLoader.LoadEnvironmentVariables();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HR Service API", Version = "v1" });
});

// Database configuration from environment variable
var connectionString = ConfigLoader.Get("HR_SQL_CONNECTION_STRING")
    ?? "Server=localhost;Database=BusinessAsUsual_HR;Trusted_Connection=True;TrustServerCertificate=True;";

var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);

if (useInMemory)
{
    Console.WriteLine("⚠️  Using in-memory database for HR Service");
    builder.Services.AddDbContext<HRDbContext>(options =>
        options.UseInMemoryDatabase("HR"));
}
else
{
    builder.Services.AddDbContext<HRDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Event bus (in-process by default, or a real broker when EventBus:Provider=Broker)
// + HR timekeeping. Time-clock punches are recorded into an in-memory (EF-ready)
// store and published as integration events; on an end-of-day punch HR closes the
// timesheet and publishes TimesheetSubmitted so Finance can hold it as pending
// payroll. HR is a publisher only, so no handlers are declared here.
builder.Services.AddEventBus(builder.Configuration, _ => { });
builder.Services.AddSingleton<TimekeepingDataStore>();
builder.Services.AddScoped<ITimekeepingService, TimekeepingService>();

// Register HTTP client for module registration
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();

// Keep the module registered (retry on startup + heartbeat to survive registry restarts)
builder.Services.AddHostedService<HR.API.Services.ModuleRegistrationHostedService>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health checks (only add DB check if not using in-memory)
var healthChecks = builder.Services.AddHealthChecks();
if (!useInMemory)
{
    healthChecks.AddDbContextCheck<HRDbContext>();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR Service API v1"));
    // HTTPS redirection only in dev; in Production the ALB terminates TLS and forwards HTTP.
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Initialize database on startup. Runs in all environments so the shared RDS
// database is created/migrated on first deploy (Production included). The
// in-memory path is used only when UseInMemoryDatabase=true.
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HRDbContext>();

        if (db.Database.IsRelational())
        {
            Console.WriteLine("Applying HR database migrations...");
            db.Database.Migrate();
            Console.WriteLine("✓ HR database migrations applied");
        }
        else
        {
            Console.WriteLine("✓ In-memory database ready for HR Service");
            db.Database.EnsureCreated();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Warning: HR database initialization failed: {ex.Message}");
    }
}

// Module registration is handled by ModuleRegistrationHostedService
// (retries on startup and re-registers periodically so the module survives a registry restart).

app.Run();

// Exposed so WebApplicationFactory<Program> can bootstrap the API in functional tests.
public partial class Program { }
