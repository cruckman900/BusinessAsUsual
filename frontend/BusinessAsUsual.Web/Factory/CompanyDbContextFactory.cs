using BusinessAsUsual.Web.Data.Context;
using BusinessAsUsual.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Web.Factory
{
    /// <summary>
    /// Provides a factory for creating instances of <see cref="CompanyDbContext"/> configured with the current
    /// session's connection settings.
    /// </summary>
    /// <remarks>This factory is designed to simplify the creation of <see cref="CompanyDbContext"/> instances
    /// by using the connection string provided by the associated <see cref="CompanySession"/>. It ensures that the <see
    /// cref="CompanyDbContext"/> is properly configured for use with the current session.</remarks>
    public class CompanyDbContextFactory
    {
        private readonly CompanySession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyDbContextFactory"/> class with the specified company
        /// session.
        /// </summary>
        /// <param name="session">The <see cref="CompanySession"/> instance that provides session-specific configuration and context for the
        /// factory. Cannot be <see langword="null"/>.</param>
        public CompanyDbContextFactory(CompanySession session)
        {
            _session = session;
        }

        /// <summary>
        /// Creates and returns a new instance of <see cref="CompanyDbContext"/> configured with the current session's
        /// connection string.
        /// </summary>
        /// <remarks>The returned <see cref="CompanyDbContext"/> is configured to use a SQL Server
        /// database. Ensure that the session provides a valid connection string.</remarks>
        /// <returns>A new instance of <see cref="CompanyDbContext"/> configured with the appropriate database connection
        /// options.</returns>
        public CompanyDbContext Create()
        {
            var options = new DbContextOptionsBuilder<CompanyDbContext>()
                .UseSqlServer(_session.GetConnectionString())
                .Options;

            return new CompanyDbContext(options);
        }
    }
}
