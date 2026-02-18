using Amazon.CloudWatch;
using BusinessAsUsual.Infrastructure.Monitoring;
using BusinessAsUsual.Web.Modules.HR.Services;
using BusinessAsUsual.Web.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using MudBlazor.Services;

namespace BusinessAsUsual.Web
{
    /// <summary>
    /// Entry point for the BusinessAsUsual.Web application.
    /// Initializes services, configures middleware, and launches the Blazor Server runtime.
    /// </summary>
    /// <remarks>
    /// Contributor: Christopher Ruckman  
    /// Created: 2025-10-05  
    /// Tags: #startup #blazor #mudblazor #server #entrypoint  
    /// </remarks>
    public static class Program
    {
        /// <summary>
        /// Configures and starts the BusinessAsUsual.Web application.
        /// Sets up Razor Components, MudBlazor services, and request pipeline middleware.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <returns>A task representing the asynchronous operation of launching the web host.</returns>
        /// <remarks>
        /// Contributor: Christopher Ruckman  
        /// Last updated: 2025-10-05  
        /// Tags: #startup #async #middleware #mudblazor #razorcomponents  
        /// </remarks>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMudServices();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            // Register ThemeContext as a singleton (global theme state)
            builder.Services.AddSingleton<ThemeContext>();

            // Register Scoped services
            builder.Services.AddScoped<CircuitHandler, LoggingCircuitHandler>();
            builder.Services.AddScoped<PageHeaderService>();

            // DI Registration
            builder.Services.AddScoped<IHRService, HRService>();

            if (builder.Environment.IsProduction())
            {
                builder.Services.AddAWSService<IAmazonCloudWatch>();
                builder.Services.AddSingleton<IMetricPublisher, CloudWatchMetricPublisher>();
            }

            builder.Services.AddControllers(); // tiny MVC controller for testing errors and metrics

            // Register Circuit Logging
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.Logging.AddConsole();

            // Register Razor Components and MudBlazor services

            var app = builder.Build();

            if (app.Environment.IsProduction())
            {
                app.UseMiddleware<RequestMetricsMiddleware>();
            }

            // Configure middleware for production environments
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            // Map static assets and Razor components
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAntiforgery();

            app.UseHttpsRedirection();

            app.MapControllers(); // Map MVC controllers for API endpoints for testing purposes
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            // Launch the application
            await app.RunAsync().ConfigureAwait(false);
        }
    }
}
