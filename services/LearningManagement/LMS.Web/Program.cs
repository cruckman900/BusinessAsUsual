using LMS.Infrastructure;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Data;
using LMS.Application;
using BusinessAsUsual.Core.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddRadzenComponents();

// Add Event Bus
builder.Services.AddInProcessEventBus();

// Add LMS services
builder.Services.AddLMSApplication();
builder.Services.AddLMSInfrastructure(builder.Configuration);

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LMSDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Ensuring database is created...");
        await context.Database.EnsureCreatedAsync();

        logger.LogInformation("Seeding database...");
        var seedData = services.GetRequiredService<LMSSeedData>();
        await seedData.SeedAsync();

        logger.LogInformation("Database initialization complete!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<LMS.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
