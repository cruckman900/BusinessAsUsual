using HR.Infrastructure.Persistence;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Admin.Extensions;

/// <summary>
/// Extension methods for initializing and seeding databases
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Initialize and seed all databases with demo data
    /// </summary>
    public static async Task InitializeAndSeedDatabasesAsync(this IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        try
        {
            // Initialize HR database
            await InitializeHRDatabaseAsync(scopedServices, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing databases.");
        }
    }

    private static async Task InitializeHRDatabaseAsync(IServiceProvider services, ILogger logger)
    {
        try
        {
            var context = services.GetService<HRDbContext>();
            if (context == null)
            {
                logger.LogWarning("HRDbContext not registered, skipping HR database initialization");
                return;
            }

            logger.LogInformation("Ensuring HR database is created...");
            await context.Database.EnsureCreatedAsync();

            logger.LogInformation("Seeding HR database...");
            var seedData = services.GetRequiredService<HRSeedData>();
            await seedData.SeedAsync();

            logger.LogInformation("HR database initialization complete!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the HR database.");
            throw;
        }
    }
}
