using BusinessAsUsual.Admin.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Admin.Data
{
    /// <summary>
    ///  Represents the EF Core context for adding companies and business logic.
    /// </summary>
    public class AdminDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options) { }

        /// <summary>Represents the companies registered in the system.</summary>
        public DbSet<Company> Companies { get; set; }
    }
}