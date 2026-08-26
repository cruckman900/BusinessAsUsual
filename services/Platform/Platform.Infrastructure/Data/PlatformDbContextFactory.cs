using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Infrastructure.Data;

/// <summary>
/// Design-time factory for PlatformDbContext to support EF Core migrations
/// </summary>
public class PlatformDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PlatformDbContext>();

        // Use SQL Server with a placeholder connection string
        // The actual connection string will be provided at runtime
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=BusinessAsUsual_Platform;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new PlatformDbContext(optionsBuilder.Options);
    }
}
