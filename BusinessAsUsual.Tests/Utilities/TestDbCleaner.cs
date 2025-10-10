using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessAsUsual.Tests.Utilities
{
    /// <summary>
    /// Provides methods to clean and reset the test database between runs.
    /// </summary>
    public class TestDBCleaner
    {
        private readonly DbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public TestDBCleaner(DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Deletes all data from tracked entities.
        /// </summary>
        public async Task CleanAsync()
        {
            var entityTypes = _context.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.GetTableName();
                var sql = $"DELETE FROM [{tableName}]";
                _logger.LogInformation("🧹 Cleaning table: {TableName}", tableName);

                await _context.Database.ExecuteSqlRawAsync(sql);
            }
        }
    }
}