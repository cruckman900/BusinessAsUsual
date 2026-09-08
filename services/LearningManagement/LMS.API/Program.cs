using LMS.Application;
using LMS.Infrastructure;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Persistence;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add event bus
builder.Services.AddInProcessEventBus();

// Add LMS services
builder.Services.AddLMSApplication();
builder.Services.AddLMSInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();
app.MapControllers();

// Initialize database and seed demo data on startup. LMSSeedData already exists
// (courses/modules/lessons/progress/certificates/notifications) but was never
// invoked before now, so the LMS database was always empty at runtime.
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LMSDbContext>();
        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<LMSSeedData>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Warning: LMS database initialization failed: {ex.Message}");
    }
}

app.Run();
