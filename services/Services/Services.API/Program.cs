using Microsoft.EntityFrameworkCore;
using Services.Infrastructure.Data;
using Services.Infrastructure.Repositories;
using Services.Domain.Interfaces;
using Services.Infrastructure.Seeding;
using BusinessAsUsual.Application.Services;
using BusinessAsUsual.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddSingleton<ITenantContextAccessor, TenantContextAccessor>();

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

app.UseMiddleware<TenantResolutionMiddleware>();

app.MapControllers();

// Dev/demo-only endpoint that clears and reseeds the Services database so a fresh,
// fully-populated "tenant" dataset is available every time a user logs in with the
// admin/password demo credentials.
app.MapPost("/api/services/tenant-reset", async (ServicesDbContext context) =>
{
    if (usingSqlServer)
    {
        return Results.BadRequest("Tenant reset is only supported for the in-memory demo database.");
    }

    context.Services.RemoveRange(context.Services);
    await context.SaveChangesAsync();

    var seeder = new DataSeeder(context);
    await seeder.SeedAsync();

    return Results.Ok(new { message = "Services tenant database reset and reseeded." });
}).AllowAnonymousTenant();

app.Run();
