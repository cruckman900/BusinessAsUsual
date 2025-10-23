using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Web.Services
{
    /// <summary>
    /// Manages the database connection string for a specific company session.
    /// </summary>
    public class CompanySession
    {
        private string? _connectionString;

        /// <summary>
        /// Set database connection string for the given database name.
        /// </summary>
        /// <param name="dbName"></param>
        public void SetDatabase(string dbName)
        {
            try
            {
                var baseConnectionString = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING");
                var builder = new SqlConnectionStringBuilder(baseConnectionString)
                {
                    InitialCatalog = dbName
                };

                _connectionString = builder.ConnectionString;

                System.Diagnostics.Debug.WriteLine($"✅ CompanySession initialized with DB: {dbName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CompanySession failed to initialize DB: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Returns the connection string for the current company session.
        /// </summary>
        /// <returns></returns>
        public string? GetConnectionString() => _connectionString;
    }
}