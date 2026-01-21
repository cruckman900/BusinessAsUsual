using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Services;
using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Admin.Database
{
    /// <summary>
    /// Handles all provisioning-related database operations:
    /// - Master DB validation
    /// - Master schema creation
    /// - Company record creation
    /// - Module provisioning
    /// - Tenant DB creation
    /// - Tenant schema application
    /// </summary>
    public class ProvisioningDb
    {
        private readonly string _rawConn;
        private readonly string _masterConn;

        /// <summary>
        /// Initializes a new instance of the ProvisioningDb class using the connection string specified in the
        /// AWS_SQL_CONNECTION_STRING environment variable.
        /// </summary>
        /// <remarks>The constructor retrieves the database connection string from the environment. Ensure
        /// that the AWS_SQL_CONNECTION_STRING environment variable is defined before creating an instance of this
        /// class.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if the AWS_SQL_CONNECTION_STRING environment variable is not set.</exception>
        public ProvisioningDb()
        {
            _rawConn = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING")
                      ?? throw new InvalidOperationException("Missing AWS_SQL_CONNECTION_STRING");

            _masterConn = _rawConn.Replace("Database=BusinessAsUsual", "Database=master");
        }

        // ------------------------------------------------------------
        // MASTER DATABASE
        // ------------------------------------------------------------

        /// <summary>
        /// Ensures that the master database named 'BusinessAsUsual' exists, creating it if it does not.
        /// </summary>
        /// <remarks>This method connects to the SQL Server instance using the master connection and
        /// checks for the existence of the 'BusinessAsUsual' database. If the database does not exist, it is created.
        /// This operation is asynchronous and should be awaited. If the database already exists, no action is
        /// taken.</remarks>
        /// <returns></returns>
        public async Task EnsureMasterDatabaseExistsAsync()
        {
            await using var conn = new SqlConnection(_masterConn);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = 'BusinessAsUsual'";

            var exists = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

            if (!exists)
            {
                var createCmd = conn.CreateCommand();
                createCmd.CommandText = "CREATE DATABASE [BusinessAsUsual]";
                await createCmd.ExecuteNonQueryAsync();

                Console.WriteLine("🟢 Created master database BusinessAsUsual");
            }
        }

        /// <summary>
        /// Applies the master database schema by executing the specified SQL script against the 'BusinessAsUsual'
        /// database.
        /// </summary>
        /// <param name="script">The SQL script to execute as the master schema. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ApplyMasterSchemaAsync(string script)
        {
            var builder = new SqlConnectionStringBuilder(_rawConn)
            {
                InitialCatalog = "BusinessAsUsual"
            };

            var executor = new SchemaExecutor();
            await executor.ExecuteScriptAsync(builder.ConnectionString, script);

            Console.WriteLine("🟢 Master schema applied");
        }

        // ------------------------------------------------------------
        // COMPANY RECORD
        // ------------------------------------------------------------

        /// <summary>
        /// Asynchronously saves the specified company information to the database.
        /// </summary>
        /// <param name="company">The company information to be saved. Cannot be null. All required properties of the company must be set.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task SaveCompanyInfoAsync(Company company)
        {
            var builder = new SqlConnectionStringBuilder(_rawConn)
            {
                InitialCatalog = "BusinessAsUsual"
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Companies 
                (Id, Name, DbName, AdminEmail, BillingPlan, ModulesEnabled, CreatedAt)
                VALUES 
                (@Id, @Name, @DbName, @AdminEmail, @BillingPlan, @ModulesEnabled, @CreatedAt)
            ";

            cmd.Parameters.AddWithValue("@Id", company.Id);
            cmd.Parameters.AddWithValue("@Name", company.Name);
            cmd.Parameters.AddWithValue("@DbName", company.DbName);
            cmd.Parameters.AddWithValue("@AdminEmail", company.AdminEmail);
            cmd.Parameters.AddWithValue("@BillingPlan", company.BillingPlan ?? "");
            cmd.Parameters.AddWithValue("@ModulesEnabled", company.ModulesEnabled ?? "");
            cmd.Parameters.AddWithValue("@CreatedAt", company.CreatedAt);

            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"🟢 Saved company record for {company.Name}");
        }

        // ------------------------------------------------------------
        // TENANT DATABASE
        // ------------------------------------------------------------

        /// <summary>
        /// Creates a new tenant database with the specified name if it does not already exist.
        /// </summary>
        /// <remarks>If a database with the specified name already exists, no action is taken. The
        /// operation is performed against the SQL Server instance specified by the master connection. This method
        /// should be awaited to ensure the database is created before proceeding.</remarks>
        /// <param name="dbName">The name of the tenant database to create. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CreateTenantDatabaseAsync(string dbName)
        {
            await using var conn = new SqlConnection(_masterConn);
            await conn.OpenAsync();

            var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
            checkCmd.Parameters.AddWithValue("@dbName", dbName);

            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

            if (!exists)
            {
                var createCmd = conn.CreateCommand();
                createCmd.CommandText = $"CREATE DATABASE [{dbName}]";
                await createCmd.ExecuteNonQueryAsync();

                Console.WriteLine($"🟢 Tenant database '{dbName}' created");
            }
        }

        /// <summary>
        /// Applies the specified database schema script to the tenant database asynchronously.
        /// </summary>
        /// <param name="dbName">The name of the tenant database to which the schema script will be applied. Cannot be null or empty.</param>
        /// <param name="script">The SQL script that defines the schema changes to apply. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ApplyTenantSchemaAsync(string dbName, string script)
        {
            var builder = new SqlConnectionStringBuilder(_rawConn)
            {
                InitialCatalog = dbName
            };

            var executor = new SchemaExecutor();
            await executor.ExecuteScriptAsync(builder.ConnectionString, script);

            Console.WriteLine($"🟢 Tenant schema applied for {dbName}");
        }
    }
}