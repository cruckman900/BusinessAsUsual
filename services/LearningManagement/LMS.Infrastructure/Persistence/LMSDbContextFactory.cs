using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LMS.Infrastructure.Persistence;

public class LMSDbContextFactory : IDesignTimeDbContextFactory<LMSDbContext>
{
    public LMSDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LMSDbContext>();

        // Use SQLite for design-time operations
        optionsBuilder.UseSqlite("Data Source=lms_designtime.db");

        return new LMSDbContext(optionsBuilder.Options);
    }
}
