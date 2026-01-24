using Amazon.CloudWatchLogs;
using BusinessAsUsual.Admin.Data;
using BusinessAsUsual.Admin.Extensions;
using BusinessAsUsual.Admin.Hubs;
using BusinessAsUsual.Admin.Logging;
using BusinessAsUsual.Admin.Services;
using BusinessAsUsual.Admin.Services.Health;
using BusinessAsUsual.Admin.Services.Logs;
using BusinessAsUsual.Infrastructure;
using CorrelationId;
using CorrelationId.DependencyInjection;
using DotNetEnv;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Amazon.Extensions.NETCore.Setup;
using BusinessAsUsual.Admin.Services.Metrics;
using BusinessAsUsual.Admin.Database;

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

                // Load environment variables from .env
                await ConfigLoader.LoadEnvironmentVariables();
                Env.Load();

                var connString = ConfigLoader.Get("AWS_SQL_CONNECTION_STRING");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddRazorPages();
                builder.Services.AddSignalR();
                builder.Services.AddDbContext<AdminDbContext>(options =>
                    options.UseSqlServer(connString));
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.SetMinimumLevel(LogLevel.Information);

                builder.Services.AddScoped<IProvisioningDb, ProvisioningDb>();
                builder.Services.AddScoped<IProvisioningService, ProvisioningService>();

                builder.Services.AddDefaultCorrelationId(options =>
                {
                    options.AddToLoggingScope = true;
                    options.UpdateTraceIdentifier = true;
                    options.RequestHeader = "X-Correlation-ID";
                    options.ResponseHeader = "X-Correlation-ID";
                });
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddHealthChecks()
                    .AddProcessAllocatedMemoryHealthCheck(512) // MB threshold
                    .AddWorkingSetHealthCheck(1024) // MB threshold
                    .AddDiskStorageHealthCheck(opt =>
                    {
                        opt.AddDrive("C:\\", 1024); // 1GB minimum free
                    })
                    .AddSqlServer(
                        connectionString: connString,
                        name: "Database",
                        failureStatus: HealthStatus.Unhealthy,
                        tags: new[] { "db", "sql" }
                    )
                    .AddCheck("Self", () => HealthCheckResult.Healthy());
                builder.Services.AddSingleton<LogQueryService>();

                //if (builder.Environment.IsDevelopment())
                //{
                //    builder.Services.AddSingleton<ILogReader, LocalLogReader>();
                //}
                //else
                //{
                //    builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
                //    builder.Services.AddAWSService<IAmazonCloudWatchLogs>();
                //    builder.Services.AddSingleton<ILogReader, CloudWatchLogReader>();
                //}
                // TEMPORARY: Always use LocalLogReader
                builder.Services.AddSingleton<ILogReader, LocalLogReader>();
                //builder.Services.AddSingleton<ILogReader, CloudWatchLogReader>();

                builder.Services.AddSingleton<EnvironmentService>();

                if (OperatingSystem.IsWindows())
                {
                    builder.Services.AddSingleton<IHealthMetricsProvider, WindowsHealthMetricsProvider>();
                }
                else if (OperatingSystem.IsLinux())
                {
                    builder.Services.AddSingleton<IHealthMetricsProvider, LinuxHealthMetricsProvider>();
                }
                else
                {
                    builder.Services.AddSingleton<IHealthMetricsProvider, LinuxHealthMetricsProvider>();
                }

                builder.Services.AddSingleton<CpuCollector>();
                builder.Services.AddSingleton<MemoryCollector>();
                builder.Services.AddSingleton<DiskCollector>();
                builder.Services.AddSingleton<NetworkCollector>();
                builder.Services.AddSingleton<UptimeCollector>();

                builder.Services.AddSingleton<SystemSettingsService>();

                // Initialize Serilog AFTER configuration is loaded
                SerilogBootstrapper.Initialize();

                // Register your services
                builder.Services.AddBusinessAsUsualServices();

                if (string.IsNullOrWhiteSpace(connString))
                {
                    Console.WriteLine("❌ AWS_SQL_CONNECTION_STRING is missing or empty.");
                }
                else
                {
                    var retries = 10;
                    while (retries > 0)
                    {
                        try
                        {
                            using var conn = new SqlConnection(connString);
                            await conn.OpenAsync();
                            Console.WriteLine("✅ SQL Server is ready.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⏳ Waiting for SQL Server... {ex.Message}");
                            Thread.Sleep(2000);
                            retries--;
                        }
                    }

                    if (retries == 0)
                    {
                        Console.WriteLine("❌ SQL Server did not become ready in time.");
                    }
                }

                var app = builder.Build();

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

                app.MapHub<ProvisioningHub>("/provisioningHub");
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

                app.UseEndpoints(static endpoints =>
                {
                    endpoints.MapHub<ProvisioningHub>("/provisioningHub");
                });

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