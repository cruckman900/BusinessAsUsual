using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HR.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by EF Core tools (e.g. <c>dotnet ef migrations</c>).
/// It always configures the SQL Server provider so migration scaffolding works
/// regardless of the runtime environment settings (such as the in-memory
/// database used for local development and tests). This factory is not used at
/// application runtime.
/// </summary>
public class HRDbContextFactory : IDesignTimeDbContextFactory<HRDbContext>
{
    public HRDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("HR_SQL_CONNECTION_STRING")
            ?? "Server=localhost;Database=BusinessAsUsual_HR;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new HRDbContext(optionsBuilder.Options);
    }
}
