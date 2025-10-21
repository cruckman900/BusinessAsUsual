using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Web.Data.Entities;

namespace BusinessAsUsual.Web.Data.Context
{

    /// <summary>
    /// Represents the database context for a company, including employee records.
    /// </summary>
    public class CompanyDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the employee records for the company.
        /// </summary>
        public DbSet<Employee> Employees { get; set; } = default!;
    }
}