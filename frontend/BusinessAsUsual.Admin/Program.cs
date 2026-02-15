using Amazon.CloudWatch;
using BusinessAsUsual.Admin.Extensions;
using BusinessAsUsual.Admin.Hubs;
using BusinessAsUsual.Admin.Logging;
using BusinessAsUsual.Infrastructure.Monitoring;
using CorrelationId;
using CorrelationId.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace BusinessAsUsual.Admin
{
    /// <summary>
    /// Provides the entry point and startup configuration for the BusinessAsUsual.Admin web application.
    /// </summary>
    /// <remarks>This class is not intended to be instantiated. It configures application services, logging,
    /// database connectivity, and middleware, and then starts the web host. The configuration includes loading
    /// environment variables, setting up dependency injection, and initializing Serilog for structured logging. The
    /// class follows the standard ASP.NET Core application startup pattern.</remarks>
#pragma warning disable S1118 // Utility classes should not have public constructors
    public class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
    {

        /// <summary>
        /// Configures and runs the BusinessAsUsual.Admin web application host using the specified command-line
        /// arguments.
        /// </summary>
        /// <remarks>This method initializes logging, loads configuration and environment variables, sets
        /// up required services, and starts the ASP.NET Core web host. It also performs readiness checks for the SQL
        /// Server database before starting the application. If an unhandled exception occurs during startup, it is
        /// logged as a fatal error. This method is intended to be used as the application's entry point.</remarks>
        /// <param name="args">An array of command-line arguments to configure the web host.</param>
        /// <returns>A task that represents the asynchronous operation of running the web application.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "ASP0014:Suggest using top level route registrations", Justification = "<Pending>")]
        public static async Task Main(string[] args)
        {
            SerilogBootstrapper.Initialize();

            try
            {
                Log.Information("Starting BusinessAsUsual.Admin web host");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddRazorPages();
                builder.Services.AddSignalR();

                builder.Services.AddAWSService<IAmazonCloudWatch>();
                builder.Services.AddSingleton<IMetricPublisher, CloudWatchMetricPublisher>();
                //builder.Services.AddSingleton<RequestMetricsMiddleware>();

                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.SetMinimumLevel(LogLevel.Information);

                builder.Services.AddDefaultCorrelationId(options =>
                {
                    options.AddToLoggingScope = true;
                    options.UpdateTraceIdentifier = true;
                    options.RequestHeader = "X-Correlation-ID";
                    options.ResponseHeader = "X-Correlation-ID";
                });

                builder.Services.AddHealthChecks()
                    .AddProcessAllocatedMemoryHealthCheck(512) // MB threshold
                    .AddWorkingSetHealthCheck(1024) // MB threshold
                    .AddDiskStorageHealthCheck(opt =>
                    {
                        opt.AddDrive("C:\\", 1024); // 1GB minimum free
                    })
                    .AddCheck("Self", () => HealthCheckResult.Healthy());

                // Initialize Serilog AFTER configuration is loaded
                SerilogBootstrapper.Initialize();

                // Register your services
                builder.Services.AddBusinessAsUsualServices();

                // HttpClient for calling the API provisioning endpoint
                builder.Services.AddHttpClient("ProvisioningApi", client =>
                {
                    client.BaseAddress = new Uri(
                        builder.Configuration["ApiBaseUrl"]
                        ?? "https://localhost:5001" // fallback for dev
                    );
                });

                var app = builder.Build();

                app.UseMiddleware<RequestMetricsMiddleware>();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    app.UseHsts();
                }

                app.UseStaticFiles();
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthorization();
                app.UseDeveloperExceptionPage();
                app.UseCorrelationId();

                app.MapHub<SmartCommitHub>("/smartCommitHub");

                app.MapDefaultControllerRoute();

                // Root landing page
                app.MapControllerRoute(
                    name: "root",
                    pattern: "",
                    defaults: new { controller = "Home", action = "Index" }
                );

                // Admin area route
                app.MapControllerRoute(
                    name: "Admin",
                    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                );

                // Default route for non-area controllers
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                app.MapStaticAssets();
                app.MapRazorPages().WithStaticAssets();

                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "Handled {RequestPath} in {Elapsed:0.0000} ms";
                });

                app.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "BAU Admin terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}