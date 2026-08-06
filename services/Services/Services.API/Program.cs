using Microsoft.EntityFrameworkCore;
using Services.Infrastructure.Data;
using Services.Infrastructure.Repositories;
using Services.Domain.Interfaces;
using Services.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure EF Core: prefer SQL Server when a connection string is provided, otherwise use InMemory fallback
var servicesConnection = builder.Configuration.GetConnectionString("Services");
var usingSqlServer = !string.IsNullOrWhiteSpace(servicesConnection);
if (usingSqlServer)
{
    builder.Services.AddDbContext<ServicesDbContext>(options =>
        options.UseSqlServer(servicesConnection));
}
else
{
    builder.Services.AddDbContext<ServicesDbContext>(options =>
        options.UseInMemoryDatabase("ServicesDb"));
}

// Register repository
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

var app = builder.Build();

// Ensure database is ready and seed sample data (useful for InMemory fallback)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ServicesDbContext>();
        if (usingSqlServer)
        {
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                // Migration failures should not prevent startup in dev scenarios
                Console.WriteLine($"[Services.API] Database migration failed: {ex.Message}");
            }
        }

        // Seed sample data if necessary
        var seeder = new DataSeeder(db);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Services.API] Failed to prepare database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
