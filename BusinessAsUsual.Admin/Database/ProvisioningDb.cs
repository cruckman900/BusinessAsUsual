using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Services;
using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Admin.Database
{
    /// <summary>
    /// Perfoms database functions for BusinessAsUsual tables and provisioned company databases.
    /// </summary>
    public class ProvisioningDb
    {
        /// <summary>
        /// Creates a provisioned company's tenant database.
        /// </summary>
        /// <param name="dbName"></param>
        public virtual async Task CreateDatabaseAsync(string dbName)
        {
            try
            {
                var constr = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING");
                var masterConn = constr?.Replace("Database=BusinessAsUsual", "Database=master");

                Console.WriteLine($"Trying to use connection string: {constr}");
                
                await using var conn = new SqlConnection(masterConn);
                await conn.OpenAsync();

                var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = $"SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
                checkCmd.Parameters.AddWithValue("@dbName", dbName);

                var result = await checkCmd.ExecuteScalarAsync();
                var exists = Convert.ToInt32(result) > 0;
                if (exists)
                {
                    return;
                }

                var createCmd = conn.CreateCommand();
                createCmd.CommandText = $@"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}')
                BEGIN
                    CREATE DATABASE [{dbName}]
                END";
                await createCmd.ExecuteNonQueryAsync();

                Console.WriteLine($"🎉 Database '{dbName}' created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CreateDatabaseAsync failed with exception: {ex}");
                throw;
            }
        }
        /// <summary>
        /// Runs the provided sql script on the provided database.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="script"></param>
        public virtual async Task ApplySchemaAsync(string dbName, string script)
        {
            var raw = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING")
                      ?? throw new InvalidOperationException("Missing AWS_SQL_CONNECTION_STRING");

            var builder = new SqlConnectionStringBuilder(raw)
            {
                InitialCatalog = dbName     // injects the correct DB name
            };

            Console.WriteLine($"🔌 Final connection string: {builder.ConnectionString}");

            var executor = new SchemaExecutor();
            await executor.ExecuteScriptAsync(builder.ConnectionString, script);
        }

        /// <summary>
        /// Saves metadata for a newly provisioned company.
        /// </summary>
        /// <param name="company">The company metadata to sae.</param>
        public virtual async Task SaveCompanyInfoAsync(Company company)
        {
            var connStr = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                INSERT INTO Companies (Id, Name, DbName, AdminEmail, CreatedAt)
                VALUES (@Id, @Name, @DbName, @AdminEmail, @CreatedAt)", conn);

            cmd.Parameters.AddWithValue("@Id", company.Id);
            cmd.Parameters.AddWithValue("@Name", company.Name);
            cmd.Parameters.AddWithValue("@DbName", company.DbName);
            cmd.Parameters.AddWithValue("@AdminEmail", company.AdminEmail);
            cmd.Parameters.AddWithValue("@BillingPlan", company.BillingPlan);
            cmd.Parameters.AddWithValue("@ModulesEnabled", company.ModulesEnabled);
            cmd.Parameters.AddWithValue("@CreatedAt", company.CreatedAt);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
