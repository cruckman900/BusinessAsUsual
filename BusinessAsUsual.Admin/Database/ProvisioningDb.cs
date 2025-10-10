using BusinessAsUsual.Admin.Models;
using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Admin.Database
{
    /// <summary>
    /// Perfoms database functions for BusinessAsUsual tables and provisioned company databases.
    /// </summary>
    public class ProvisioningDb
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="config"></param>
        public ProvisioningDb(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Creates a provisioned company's tenant database.
        /// </summary>
        /// <param name="dbName"></param>
        public async Task CreateDatabaseAsync(string dbName)
        {
            var masterConn = _config["ConnectionStrings:DefaultConnection"]?
                .Replace("Database=BusinessAsUsual", "Database=master");

            await using var conn = new SqlConnection(masterConn);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE [{dbName}]";
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Runs the provided sql script on the provided database.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="script"></param>
        public async Task ApplySchemaAsync(string dbName, string script)
        {
            var tenantConn = _config["ConnectionStrings:DefaultConnection"]?
                .Replace("Database=BusinessAsUsual", $"Database={dbName}");

            await using var conn = new SqlConnection(tenantConn);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = script;
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Saves metadata for a newly provisioned company.
        /// </summary>
        /// <param name="company">The company metadata to sae.</param>
        public async Task SaveCompanyInfoAsync(Company company)
        {
            var connStr = _config.GetConnectionString("DefaultConnection");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                INSERT INTO Companies (Name, DbName, AdminEmail, CreatedAt)
                VALUES (@Id, @Name, @DbName, @AdminEmail, @CreatedAt)", conn);

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
