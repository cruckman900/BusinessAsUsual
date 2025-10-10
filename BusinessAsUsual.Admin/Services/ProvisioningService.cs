using Azure.Identity;
using BusinessAsUsual.Admin.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BusinessAsUsual.Admin.Services
{
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
    /// <param name="hub"></param>
    /// <param name="config"></param>
    /// <param name="env"></param>
    /// <param name="metadataService"></param>
    /// <param name="logger"></param>
    public class ProvisioningService(
        IHubContext<ProvisioningHub> hub,
        IConfiguration config,
        IHostEnvironment env,
        TenantMetadataService metadataService,
        ILogger<ProvisioningService> logger) : IProvisioningService
    {
        private readonly IHubContext<ProvisioningHub> _hub = hub;
        private readonly IConfiguration _config = config;
        private readonly IHostEnvironment _env = env;
        private readonly TenantMetadataService _metadataService = metadataService;
        private readonly ILogger<ProvisioningService> _logger = logger;

        static private async Task LogProvisioningStepAsync(SqlConnection connection, Guid companyId, string step, string status, string message)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO ProvisioningLog (Id, CompanyId, Step, Status, Message, Timestamp)
                VALUES (@Id, @CompanyId, @Step, @Status, @Message, GETUTCDATE())";

            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@CompanyId", companyId);
            command.Parameters.AddWithValue("@Step", step);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Message", message);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Provisions a new company by creating metadata, tenant database, and schema.
        /// </summary>
        public async Task<bool> ProvisionTenantAsync(string tenantName, string adminEmail, string billingPlan, string[] modules)
        {
            var companyId = Guid.NewGuid();
            var dbName = $"bau_{tenantName.ToLower().Replace(" ", "_")}";
            var masterConnStr = _config.GetConnectionString("DefaultConnection");

            await using var masterConnection = new SqlConnection(masterConnStr);
            await masterConnection.OpenAsync();
            await using var transaction = await masterConnection.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("🚀 [Provisioning] Starting tenant provisioning for '{TenantName}'", tenantName);
                await _hub.Clients.All.SendAsync("ReceiveStatus", "🚀 [Provisioning] Starting tenant provisioning for '{TenantName}'", tenantName);
                // Step 1: Create metadata schema if needed
                _logger.LogInformation("📦 [Provisioning] Creating metadata for '{TenantName}'...", tenantName);
                var metadataScriptPath = Path.Combine(_env.ContentRootPath, "ProvisioningScripts", "CreateCompanyMetadata.sql");
                var metadataScript = await File.ReadAllTextAsync(metadataScriptPath);
                await new SqlCommand(metadataScript, masterConnection, (SqlTransaction)transaction).ExecuteNonQueryAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "CreateMetadataSchema", "Success", "Metadata schema ensured.");
                _logger.LogInformation("✅ [Provisioning] Metadata created successfully");
                await _hub.Clients.All.SendAsync("ReceiveStatus", "✅ [Provisioning] Metadata created successfully");

                // Step 2: Create tenant database
                _logger.LogInformation("📦 [Provisioning] Creating database ({Database})", dbName);
                await _hub.Clients.All.SendAsync("ReceiveStatus", "📦 [Provisioning] Creating database ({Database})", dbName);
                var createDbCommand = $"CREATE DATABASE [{dbName}]";
                await new SqlCommand(createDbCommand, masterConnection, (SqlTransaction)transaction).ExecuteNonQueryAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "CreateTenantDatabase", "Success", $"Database '{dbName}' created.");
                _logger.LogInformation("✅ [Provisioning] Database created successfully");
                await _hub.Clients.All.SendAsync("ReceiveStatus", "✅ [Provisioning] Database created successfully");

                // Step 3: Run tenant schema
                _logger.LogInformation("📦 [Provisioning] Creating schema for '{TenantName}'...", tenantName);
                await _hub.Clients.All.SendAsync("ReceiveStatus", "📦 [Provisioning] Creating schema for '{TenantName}'...", tenantName);
                var tenantConnStr = masterConnStr!.Replace("Database=BusinessAsUsual", $"Database={dbName}");
                await using var tenantConnection = new SqlConnection(tenantConnStr);
                await tenantConnection.OpenAsync();
                await LogProvisioningStepAsync(tenantConnection, companyId, "RunTenantSchema", "Success", "Default schema applied.");

                var tenantScriptPath = Path.Combine(_env.ContentRootPath, "ProvisioningScripts", "DefaultSchema.sql");
                var tenantScript = await File.ReadAllTextAsync(tenantScriptPath);
                await new SqlCommand(tenantScript, tenantConnection).ExecuteNonQueryAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "SaveMetadata", "Success", "Company metadata saved.");
                _logger.LogInformation("✅ [Provisioning] Schema created successfully");
                await _hub.Clients.All.SendAsync("ReceiveStatus", "✅ [Provisioning] Schema created successfully✅ [Provisioning] Schema created successfully");

                // Step 4: Save metadata
                _logger.LogInformation("📦 [Provisioning] Saving metadata for {TenantName}", tenantName);
                await _hub.Clients.All.SendAsync("ReceiveStatus", "📦 [Provisioning] Saving metadata for {TenantName}", tenantName);
                var company = new Company
                {
                    Id = companyId,
                    Name = tenantName,
                    DbName = dbName,
                    AdminEmail = adminEmail,
                    BillingPlan = billingPlan,
                    ModulesEnabled = string.Join(",", modules),
                    CreatedAt = DateTime.UtcNow
                };

                await _metadataService.SaveAsync(company);
                await transaction.CommitAsync();
                _logger.LogInformation("✅ [Provisioning] Metadata saved successfully");
                await _hub.Clients.All.SendAsync("ReceiveStatus", "✅ [Provisioning] Metadata saved successfully");

                _logger.LogInformation("🎉 [Provisioning] Tenant '{TenantName}' provisioned successfully", tenantName);
                await _hub.Clients.All.SendAsync("ReceiveStatus", "🎉 [Provisioning] Tenant '{TenantName}' provisioned successfully", tenantName);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "Provisioning", "Failed", ex.Message);
                _logger.LogError(ex, "❌ [Provisioning] Failed for company '{TenantName}'", tenantName);
                await _hub.Clients.All.SendAsync("ReceiveStatus", "❌ [Provisioning] Failed for company '{TenantName}'", tenantName);
                return false;
            }
        }
    }
}