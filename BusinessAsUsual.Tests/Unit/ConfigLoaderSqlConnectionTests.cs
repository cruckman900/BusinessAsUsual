using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace BusinessAsUsual.Tests
{
    public class ConfigLoaderSqlConnectionTests
    {
        [Fact]
        public async Task Should_ConnectToSqlServer_WithHardcodedConnectionString()
        {
            // Load environment (optional sanity check)
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Unknown";
            Console.WriteLine($"🧪 Environment: {env}");

            // Hardcoded connection string for local dev
            var connectionString = "Server=localhost,1433;User Id=sa;Password=YourStrong!Password123;Database=BusinessAsUsual;Encrypt=True;TrustServerCertificate=true;";

            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                Console.WriteLine("✅ SQL Server connection succeeded.");
                Assert.True(conn.State == System.Data.ConnectionState.Open);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SQL Server connection failed: {ex.Message}");
                throw;
            }
        }
    }
}