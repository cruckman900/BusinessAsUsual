using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ModuleRegistry.Application.Services;
using ModuleRegistry.Domain.Repositories;
using ModuleRegistry.Infrastructure.Persistence;
using ModuleRegistry.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Module Registry Service API", Version = "v1" });
});

// Database configuration
var connectionString = Environment.GetEnvironmentVariable("MRS_SQL_CONNECTION_STRING")
    ?? "Server=localhost;Database=BusinessAsUsual_ModuleRegistry;Trusted_Connection=True;TrustServerCertificate=True;";

var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);

if (useInMemory)
{
    Console.WriteLine("⚠️  Using in-memory database for development");
    builder.Services.AddDbContext<ModuleRegistryDbContext>(options =>
        options.UseInMemoryDatabase("ModuleRegistry"));
}
else
{
    builder.Services.AddDbContext<ModuleRegistryDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register repositories
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();

// Register services
builder.Services.AddScoped<IModuleRegistryService, ModuleRegistryService>();

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
    healthChecks.AddDbContextCheck<ModuleRegistryDbContext>();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Module Registry Service API v1"));
    // HTTPS redirection only in dev; in Production the ALB terminates TLS and forwards HTTP.
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Initialize database on startup in ALL environments so the shared RDS database
// is created/migrated on first deploy (Production included). In-memory is used
// only when UseInMemoryDatabase=true.
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ModuleRegistryDbContext>();

        if (useInMemory)
        {
            Console.WriteLine("✓ In-memory database ready");
            db.Database.EnsureCreated();
        }
        else
        {
            Console.WriteLine("Applying ModuleRegistry database migrations...");
            db.Database.Migrate();
            Console.WriteLine("✓ ModuleRegistry database migrations applied");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Warning: Database initialization failed: {ex.Message}");
        Console.WriteLine("   The service will start but database operations will fail.");
        Console.WriteLine("   Make sure SQL Server/RDS is reachable or set UseInMemoryDatabase=true.");
    }
}

app.Run();

// Exposed so WebApplicationFactory<Program> can bootstrap the API in functional tests.
public partial class Program { }
