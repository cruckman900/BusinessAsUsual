using BusinessAsUsual.Admin.Database;
using BusinessAsUsual.Admin.Areas.Admin.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// ProvisioningLog table logger class.
    /// </summary>
    public class ProvisioningLogger
    {
        private readonly IHubContext<ProvisioningHub> _hub;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="hub"></param>
        public ProvisioningLogger(IHubContext<ProvisioningHub> hub)
        {
            _hub = hub;
        }

        /// <summary>
        /// Log provisioning setps to the ProvisioningLog table in the BusinessAsUsual datebase.
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="step"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task LogStepAsync(string tenantName, string step, string status, string message)
        {
            await _hub.Clients.All.SendAsync("Log", new { tenantName, step, status, message });

            var connStr = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Missing AWS_SQL_CONNECTION_STRING");
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var command = conn.CreateCommand();
            command.CommandText = @"
            INSERT INTO ProvisioningLog (TenantName, Step, Status, Message, Timestamp)
            VALUES (@Tenantname, @Step, @Status, @Message, GETUTCDATE())";

            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@TenantName", tenantName);
            command.Parameters.AddWithValue("@Step", step);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Message", message);

            await command.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// SignalR hub that handles messaging to the client.
    /// </summary>
    public class ProvisioningHub: Hub
    {
        /// <summary>
        /// Broadcast log message to the client.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendStatus(string message)
        {
            await Clients.All.SendAsync("ReceiveStatus", message);
        }
    }

    /// <summary>
    /// Handles the provisioning of new company databases and schema setup.
    /// </summary>
    /// <remarks>
    /// Default constructor for CompanyProvisioner
    /// </remarks>
    /// <param name="env"></param>
    /// <param name="metadataService"></param>
    /// <param name="logger"></param>
    /// <param name="provDb"></param>
    /// <param name="provLogger"></param>
    public class ProvisioningService(
        IHostEnvironment env,
        TenantMetadataService metadataService,
        ILogger<ProvisioningService> logger,
        ProvisioningDb provDb,
        ProvisioningLogger provLogger) : IProvisioningService
    {
        private readonly IHostEnvironment _env = env;
        private readonly TenantMetadataService _metadataService = metadataService;
        private readonly ILogger<ProvisioningService> _logger = logger;
        private readonly ProvisioningDb _provDb = provDb;
        private readonly ProvisioningLogger _provLogger = provLogger;

        /// <summary>
        /// Provisions a new company by creating metadata, tenant database, and schema.
        /// </summary>
        public async Task<bool> ProvisionTenantAsync(
            string tenantName,
            string adminEmail,
            string billingPlan,
            string[] modules)
        {
            var dbName = $"bau_{tenantName.ToLower().Replace(" ", "_")}";

            try
            {
                // 🪵 Ensure ProvisioningLog table exists in default DB
                await _provDb.ApplySchemaAsync("BusinessAsUsual", _metadataService.GetProvisioningLogScript());

                // 🏢 Ensure Companies registry table exists in default DB
                await _provDb.ApplySchemaAsync("BusinessAsUsual", _metadataService.GetCompanyRegistryScript());

                // 🧾 Save company metadata to global registry
                var company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = tenantName,
                    DbName = dbName,
                    AdminEmail = adminEmail,
                    BillingPlan = billingPlan,
                    ModulesEnabled = string.Join(",", modules),
                    CreatedAt = DateTime.UtcNow
                };

                await _provDb.SaveCompanyInfoAsync(company);

                // 📡 Log start of provisioning
                await _provLogger.LogStepAsync(tenantName, "Provisioning", "Started", $"Creating database {dbName}");

                // 🏗️ Create tenant database
                await _provDb.CreateDatabaseAsync(dbName);

                // 🧱 Apply tenant schema from DefaultSchema.sql
                await _provDb.ApplySchemaAsync(dbName, _metadataService.GetCreateScript(_env, tenantName));

                // ✅ Log success
                await _provLogger.LogStepAsync(tenantName, "Provisioning", "Success", $"Tenant database {dbName} created and schema applied");

                return true;
            }
            catch (Exception ex)
            {
                // ❌ Log failure
                await _provLogger.LogStepAsync(tenantName, "Provisioning", "Failed", ex.Message);
                _logger.LogError(ex, "Provisioning failed for tenant {Tenant}", tenantName);
                return false;
            }
        }
    }
}