using BusinessAsUsual.Application.Database;
using BusinessAsUsual.Domain.Entities;
using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Infrastructure.Database
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
    public class ProvisioningDb : IProvisioningDb
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

            // Ensure Companies table exists with proper schema
            await EnsureCompaniesTableExistsAsync();
        }

        /// <summary>
        /// Ensures the Companies table exists in the master database with the proper schema including ModuleConfiguration column.
        /// </summary>
        private async Task EnsureCompaniesTableExistsAsync()
        {
            var builder = new SqlConnectionStringBuilder(_rawConn)
            {
                InitialCatalog = "BusinessAsUsual"
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Companies')
                BEGIN
                    CREATE TABLE Companies (
                        Id UNIQUEIDENTIFIER PRIMARY KEY,
                        Name NVARCHAR(100) NOT NULL,
                        DbName NVARCHAR(100) NOT NULL,
                        Description NVARCHAR(500),
                        AdminEmail NVARCHAR(255) NOT NULL,
                        BillingPlan NVARCHAR(50) NOT NULL,
                        ModulesEnabled NVARCHAR(MAX),
                        SubmodulesEnabled NVARCHAR(MAX),
                        ModuleConfiguration NVARCHAR(MAX),
                        IsActive BIT NOT NULL DEFAULT 1,
                        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
                    );
                END
                ELSE
                BEGIN
                    -- Add ModuleConfiguration column if it doesn't exist (migration support)
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID('Companies') 
                        AND name = 'ModuleConfiguration'
                    )
                    BEGIN
                        ALTER TABLE Companies ADD ModuleConfiguration NVARCHAR(MAX);
                    END

                    -- Add SubmodulesEnabled column if it doesn't exist (migration support)
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID('Companies') 
                        AND name = 'SubmodulesEnabled'
                    )
                    BEGIN
                        ALTER TABLE Companies ADD SubmodulesEnabled NVARCHAR(MAX);
                    END
                END
            ";

            await cmd.ExecuteNonQueryAsync();
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
                (Id, Name, DbName, AdminEmail, BillingPlan, ModulesEnabled, SubmodulesEnabled, ModuleConfiguration, CreatedAt)
                VALUES 
                (@Id, @Name, @DbName, @AdminEmail, @BillingPlan, @ModulesEnabled, @SubmodulesEnabled, @ModuleConfiguration, @CreatedAt)
            ";

            cmd.Parameters.AddWithValue("@Id", company.Id);
            cmd.Parameters.AddWithValue("@Name", company.Name);
            cmd.Parameters.AddWithValue("@DbName", company.DbName);
            cmd.Parameters.AddWithValue("@AdminEmail", company.AdminEmail);
            cmd.Parameters.AddWithValue("@BillingPlan", company.BillingPlan ?? "");
            cmd.Parameters.AddWithValue("@ModulesEnabled", (object?)company.ModulesEnabled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubmodulesEnabled", (object?)company.SubmodulesEnabled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ModuleConfiguration", (object?)company.ModuleConfiguration ?? DBNull.Value);
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

        // ------------------------------------------------------------
        // MODULE CONFIGURATION
        // ------------------------------------------------------------

        /// <summary>
        /// Saves the module configuration JSON to the tenant's ModuleRegistry table.
        /// </summary>
        /// <param name="tenantDbName">Name of the tenant database.</param>
        /// <param name="companyId">Unique identifier of the company.</param>
        /// <param name="moduleConfigJson">JSON string containing the module configuration.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task SaveModuleConfigurationToTenantAsync(string tenantDbName, Guid companyId, string moduleConfigJson)
        {
            var builder = new SqlConnectionStringBuilder(_rawConn)
            {
                InitialCatalog = tenantDbName
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF EXISTS (SELECT 1 FROM ModuleRegistry WHERE CompanyId = @CompanyId)
                BEGIN
                    UPDATE ModuleRegistry 
                    SET ModuleConfiguration = @ModuleConfiguration, UpdatedAt = GETUTCDATE()
                    WHERE CompanyId = @CompanyId
                END
                ELSE
                BEGIN
                    INSERT INTO ModuleRegistry (Id, CompanyId, ModuleConfiguration, UpdatedAt)
                    VALUES (NEWID(), @CompanyId, @ModuleConfiguration, GETUTCDATE())
                END
            ";

            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.AddWithValue("@ModuleConfiguration", moduleConfigJson);

            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"🟢 Module configuration saved to tenant {tenantDbName}");
        }

        /// <summary>
        /// Retrieves the module configuration JSON for a specific tenant.
        /// </summary>
        /// <param name="tenantDbName">Name of the tenant database.</param>
        /// <param name="companyId">Unique identifier of the company.</param>
        /// <returns>The module configuration JSON string, or null if not found.</returns>
        public async Task<string?> GetModuleConfigurationAsync(string tenantDbName, Guid companyId)
        {
            var builder = new SqlConnectionStringBuilder(_rawConn)
            {
                InitialCatalog = tenantDbName
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT ModuleConfiguration 
                FROM ModuleRegistry 
                WHERE CompanyId = @CompanyId
            ";

            cmd.Parameters.AddWithValue("@CompanyId", companyId);

            var result = await cmd.ExecuteScalarAsync();
            return result as string;
        }

        /// <summary>
        /// Executes an arbitrary SQL script against a specific tenant database.
        /// Used for module-specific schema provisioning scripts.
        /// </summary>
        /// <param name="tenantDbName">Name of the tenant database.</param>
        /// <param name="script">SQL script to execute.</param>
        /// <returns>A task that represents the asynchronous execution operation.</returns>
        public async Task ExecuteScriptAsync(string tenantDbName, string script)
        {
            var builder = new SqlConnectionStringBuilder(_rawConn)
            {
                InitialCatalog = tenantDbName
            };

            var executor = new SchemaExecutor();
            await executor.ExecuteScriptAsync(builder.ConnectionString, script);

            Console.WriteLine($"🟢 Script executed successfully on {tenantDbName}");
        }
    }
}