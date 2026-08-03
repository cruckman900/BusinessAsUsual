using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BusinessAsUsual.Admin.Tests.Integration;

/// <summary>
/// Custom test factory that configures the Admin application for integration testing.
/// </summary>
public class CustomAdminApplicationFactory : WebApplicationFactory<BusinessAsUsual.Admin.ProgramAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use Development environment to ensure all services are configured properly
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            // Remove any problematic health checks and replace with simple ones
            var healthCheckDescriptors = services
                .Where(d => d.ServiceType == typeof(IHealthCheck))
                .ToList();

            foreach (var descriptor in healthCheckDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add simple passing health check for tests
            services.AddHealthChecks()
                .AddCheck("test-health", () => HealthCheckResult.Healthy("Test environment"));
        });

        // Suppress Serilog errors in test environment
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning); // Reduce noise in tests
        });

        base.ConfigureWebHost(builder);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Override to catch and log any startup exceptions
        try
        {
            return base.CreateHost(builder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create test host: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
