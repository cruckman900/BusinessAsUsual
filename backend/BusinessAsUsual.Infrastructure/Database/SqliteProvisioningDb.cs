using System.Text.RegularExpressions;
using BusinessAsUsual.Application.Database;
using BusinessAsUsual.Domain.Entities;
using Microsoft.Data.Sqlite;

namespace BusinessAsUsual.Infrastructure.Database
{
    /// <summary>
    /// SQLite-backed implementation of <see cref="IProvisioningDb"/>, used as a fallback when no SQL Server
    /// instance is available (e.g. local demo/video recording environments). Each tenant is provisioned as
    /// its own SQLite database file under the configured tenants directory, and a shared master SQLite
    /// database file holds the Companies directory.
    /// </summary>
    /// <remarks>
    /// This implementation performs a best-effort translation of the T-SQL provisioning scripts (written for
    /// SQL Server) into SQLite-compatible statements. Only CREATE TABLE / CREATE INDEX statements are
    /// translated; foreign-key ALTER TABLE statements are skipped since SQLite cannot add FK constraints to an
    /// existing table (the tenant schema still works correctly without them for demo purposes).
    /// </remarks>
    public class SqliteProvisioningDb : IProvisioningDb
    {
        private readonly string _dataDirectory;
        private readonly string _masterDbPath;

        public SqliteProvisioningDb(string? dataDirectory = null)
        {
            _dataDirectory = dataDirectory
                ?? Path.Combine(AppContext.BaseDirectory, "App_Data", "Tenants");

            Directory.CreateDirectory(_dataDirectory);
            _masterDbPath = Path.Combine(_dataDirectory, "Master.db");
        }

        private string GetTenantDbPath(string dbName) => Path.Combine(_dataDirectory, $"{dbName}.db");

        private string BuildConnectionString(string dbPath) => new SqliteConnectionStringBuilder
        {
            DataSource = dbPath
        }.ToString();

        public async Task EnsureMasterDatabaseExistsAsync()
        {
            await using var conn = new SqliteConnection(BuildConnectionString(_masterDbPath));
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Companies (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    DbName TEXT NOT NULL,
                    Description TEXT,
                    AdminEmail TEXT NOT NULL,
                    BillingPlan TEXT NOT NULL,
                    ModulesEnabled TEXT,
                    SubmodulesEnabled TEXT,
                    ModuleConfiguration TEXT,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );";
            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine("🟢 (SQLite) Verified master database Companies table");
        }

        public async Task SaveCompanyInfoAsync(Company company)
        {
            await using var conn = new SqliteConnection(BuildConnectionString(_masterDbPath));
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Companies
                (Id, Name, DbName, Description, AdminEmail, BillingPlan, ModulesEnabled, SubmodulesEnabled, ModuleConfiguration, CreatedAt)
                VALUES
                (@Id, @Name, @DbName, @Description, @AdminEmail, @BillingPlan, @ModulesEnabled, @SubmodulesEnabled, @ModuleConfiguration, @CreatedAt)";

            cmd.Parameters.AddWithValue("@Id", company.Id.ToString());
            cmd.Parameters.AddWithValue("@Name", company.Name);
            cmd.Parameters.AddWithValue("@DbName", company.DbName);
            cmd.Parameters.AddWithValue("@Description", (object?)company.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AdminEmail", company.AdminEmail);
            cmd.Parameters.AddWithValue("@BillingPlan", company.BillingPlan ?? "");
            cmd.Parameters.AddWithValue("@ModulesEnabled", (object?)company.ModulesEnabled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubmodulesEnabled", (object?)company.SubmodulesEnabled ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ModuleConfiguration", (object?)company.ModuleConfiguration ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", company.CreatedAt.ToString("o"));

            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"🟢 (SQLite) Saved company record for {company.Name}");
        }

        public Task CreateTenantDatabaseAsync(string dbName)
        {
            // Creating the SqliteConnection and opening it is enough to create the file.
            var dbPath = GetTenantDbPath(dbName);
            using var conn = new SqliteConnection(BuildConnectionString(dbPath));
            conn.Open();

            Console.WriteLine($"🟢 (SQLite) Tenant database '{dbName}' created at {dbPath}");
            return Task.CompletedTask;
        }

        public async Task ApplyTenantSchemaAsync(string dbName, string script)
        {
            await ExecuteScriptAsync(dbName, script);
            Console.WriteLine($"🟢 (SQLite) Tenant schema applied for {dbName}");
        }

        public async Task SaveModuleConfigurationToTenantAsync(string tenantDbName, Guid companyId, string moduleConfigJson)
        {
            var dbPath = GetTenantDbPath(tenantDbName);
            await using var conn = new SqliteConnection(BuildConnectionString(dbPath));
            await conn.OpenAsync();

            var ensureCmd = conn.CreateCommand();
            ensureCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ModuleRegistry (
                    Id TEXT PRIMARY KEY,
                    CompanyId TEXT NOT NULL,
                    ModuleConfiguration TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );";
            await ensureCmd.ExecuteNonQueryAsync();

            var upsertCmd = conn.CreateCommand();
            upsertCmd.CommandText = @"
                INSERT INTO ModuleRegistry (Id, CompanyId, ModuleConfiguration, UpdatedAt)
                VALUES (@Id, @CompanyId, @ModuleConfiguration, @UpdatedAt)
                ON CONFLICT(Id) DO UPDATE SET
                    ModuleConfiguration = excluded.ModuleConfiguration,
                    UpdatedAt = excluded.UpdatedAt;";

            // Emulate "update if exists for this CompanyId, else insert" since SQLite ON CONFLICT needs a unique key.
            var existingIdCmd = conn.CreateCommand();
            existingIdCmd.CommandText = "SELECT Id FROM ModuleRegistry WHERE CompanyId = @CompanyId LIMIT 1";
            existingIdCmd.Parameters.AddWithValue("@CompanyId", companyId.ToString());
            var existingId = (string?)await existingIdCmd.ExecuteScalarAsync();

            upsertCmd.Parameters.AddWithValue("@Id", existingId ?? Guid.NewGuid().ToString());
            upsertCmd.Parameters.AddWithValue("@CompanyId", companyId.ToString());
            upsertCmd.Parameters.AddWithValue("@ModuleConfiguration", moduleConfigJson);
            upsertCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow.ToString("o"));

            await upsertCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"🟢 (SQLite) Module configuration saved to tenant {tenantDbName}");
        }

        public async Task<string?> GetModuleConfigurationAsync(string tenantDbName, Guid companyId)
        {
            var dbPath = GetTenantDbPath(tenantDbName);
            await using var conn = new SqliteConnection(BuildConnectionString(dbPath));
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ModuleConfiguration FROM ModuleRegistry WHERE CompanyId = @CompanyId LIMIT 1";
            cmd.Parameters.AddWithValue("@CompanyId", companyId.ToString());

            var result = await cmd.ExecuteScalarAsync();
            return result as string;
        }

        public async Task ExecuteScriptAsync(string tenantDbName, string script)
        {
            var dbPath = GetTenantDbPath(tenantDbName);
            await using var conn = new SqliteConnection(BuildConnectionString(dbPath));
            await conn.OpenAsync();

            foreach (var statement in TSqlToSqliteTranslator.Translate(script))
            {
                try
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = statement;
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    // Best-effort translation for demo purposes; log and continue rather than aborting
                    // the whole provisioning flow over one unsupported statement.
                    Console.WriteLine($"⚠️ (SQLite) Skipped unsupported statement: {ex.Message}");
                }
            }
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            await using var conn = new SqliteConnection(BuildConnectionString(_masterDbPath));
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, DbName, AdminEmail, BillingPlan FROM Companies ORDER BY Name";

            var companies = new List<Company>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                companies.Add(new Company
                {
                    Id = Guid.Parse(reader.GetString(0)),
                    Name = reader.GetString(1),
                    DbName = reader.GetString(2),
                    AdminEmail = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    BillingPlan = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }

            return companies;
        }
    }

    /// <summary>
    /// Best-effort translator that extracts CREATE TABLE / CREATE INDEX statements from the SQL Server
    /// provisioning scripts and converts common T-SQL types/functions into SQLite-compatible equivalents.
    /// Foreign key ALTER TABLE statements are intentionally skipped.
    /// </summary>
    internal static class TSqlToSqliteTranslator
    {
        public static IEnumerable<string> Translate(string script)
        {
            var statements = new List<string>();

            foreach (Match match in Regex.Matches(
                script,
                @"CREATE TABLE\s+(\w+)\s*\(([\s\S]*?)\)\s*;",
                RegexOptions.IgnoreCase))
            {
                var tableName = match.Groups[1].Value;
                var columnsRaw = match.Groups[2].Value;
                var columns = TranslateColumns(columnsRaw);
                statements.Add($"CREATE TABLE IF NOT EXISTS {tableName} ({columns});");
            }

            foreach (Match match in Regex.Matches(
                script,
                @"CREATE INDEX\s+(\w+)\s+ON\s+(\w+)\s*\(([^)]*)\)\s*;",
                RegexOptions.IgnoreCase))
            {
                var indexName = match.Groups[1].Value;
                var tableName = match.Groups[2].Value;
                var cols = match.Groups[3].Value;
                statements.Add($"CREATE INDEX IF NOT EXISTS {indexName} ON {tableName}({cols});");
            }

            return statements;
        }

        private static string TranslateColumns(string columnsRaw)
        {
            var lines = columnsRaw
                .Split(',')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .Select(TranslateColumnLine)
                .ToList();

            return string.Join(", ", lines);
        }

        private static string TranslateColumnLine(string line)
        {
            var result = line;

            result = Regex.Replace(result, @"UNIQUEIDENTIFIER", "TEXT", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"NVARCHAR\s*\(\s*MAX\s*\)", "TEXT", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"NVARCHAR\s*\(\s*\d+\s*\)", "TEXT", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"DATETIME", "TEXT", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\bBIT\b", "INTEGER", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"DECIMAL\s*\(\s*\d+\s*,\s*\d+\s*\)", "REAL", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\bDATE\b", "TEXT", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"GETUTCDATE\s*\(\s*\)", "(datetime('now'))", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"DEFAULT\s+NEWID\s*\(\s*\)", "", RegexOptions.IgnoreCase);

            return result;
        }
    }
}
