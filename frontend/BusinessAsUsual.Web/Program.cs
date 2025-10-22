using BusinessAsUsual.Infrastructure;
using BusinessAsUsual.Web.Data.Context;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

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
            // Load environment variables from .env
            ConfigLoader.LoadEnvironmentVariables();
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddDbContext<CompanyDbContext>(options =>
                options.UseSqlServer(
                    ConfigLoader.Get("AWS_SQL_CONNECTION_STRING")));
            // Register Razor Components and MudBlazor services

            var app = builder.Build();

            // Configure middleware for production environments
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseHttpsRedirection();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            // Map static assets and Razor components
            app.MapStaticAssets();

            // Launch the application
            await app.RunAsync().ConfigureAwait(false);
        }
    }
}
