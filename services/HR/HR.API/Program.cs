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

// Register HTTP client for module registration
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();

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
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Auto-migrate database on startup (development only)
if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HRDbContext>();

        if (useInMemory)
        {
            Console.WriteLine("✓ In-memory database ready for HR Service");
            db.Database.EnsureCreated();
        }
        else
        {
            Console.WriteLine("Checking HR database connection...");
            if (db.Database.CanConnect())
            {
                Console.WriteLine("✓ HR database connection successful");
                Console.WriteLine("Applying migrations...");
                db.Database.Migrate();
                Console.WriteLine("✓ HR database migrations applied");
            }
            else
            {
                Console.WriteLine("⚠️  Warning: Cannot connect to HR database. Set UseInMemoryDatabase=true in appsettings.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Warning: HR database initialization failed: {ex.Message}");
    }
}

// Register with Module Registry Service
using (var scope = app.Services.CreateScope())
{
    var registrationService = scope.ServiceProvider.GetRequiredService<IModuleRegistrationService>();
    await registrationService.RegisterWithModuleRegistryAsync();
}

app.Run();
