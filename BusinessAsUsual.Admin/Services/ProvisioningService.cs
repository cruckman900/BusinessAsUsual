using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BusinessAsUsual.Admin.Models;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Handles the provisioning of new company databases and schema setup.
    /// </summary>
    public class ProvisioningService : IProvisioningService
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;
        private readonly TenantMetadataService _metadataService;
        private readonly ILogger<ProvisioningService> _logger;

        /// <summary>
        /// Default constructor for CompanyProvisioner
        /// </summary>
        /// <param name="config"></param>
        /// <param name="env"></param>
        /// <param name="metadataService"></param>
        /// <param name="logger"></param>
        public ProvisioningService(
            IConfiguration config,
            IHostEnvironment env,
            TenantMetadataService metadataService,
            ILogger<ProvisioningService> logger)
        {
            _config = config;
            _env = env;
            _metadataService = metadataService;
            _logger = logger;
        }

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
        public async Task<bool> ProvisionTenantAsync(string companyName, string adminEmail, string billingPlan, string[] modules)
        {
            var companyId = Guid.NewGuid();
            var dbName = $"bau_{companyName.ToLower().Replace(" ", "_")}";
            var masterConnStr = _config.GetConnectionString("DefaultConnection");

            await using var masterConnection = new SqlConnection(masterConnStr);
            await masterConnection.OpenAsync();
            await using var transaction = await masterConnection.BeginTransactionAsync();

            try
            {
                // Step 1: Create metadata schema if needed
                var metadataScriptPath = Path.Combine(_env.ContentRootPath, "ProvisioningScripts", "CreateCompanyMetadata.sql");
                var metadataScript = await File.ReadAllTextAsync(metadataScriptPath);
                await new SqlCommand(metadataScript, masterConnection, (SqlTransaction)transaction).ExecuteNonQueryAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "CreateMetadataSchema", "Success", "Metadata schema ensured.");

                // Step 2: Create tenant database
                var createDbCommand = $"CREATE DATABASE [{dbName}]";
                await new SqlCommand(createDbCommand, masterConnection, (SqlTransaction)transaction).ExecuteNonQueryAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "CreateTenantDatabase", "Success", $"Database '{dbName}' created.");

                // Step 3: Run tenant schema
                var tenantConnStr = masterConnStr!.Replace("Database=BusinessAsUsual", $"Database={dbName}");
                await using var tenantConnection = new SqlConnection(tenantConnStr);
                await tenantConnection.OpenAsync();
                await LogProvisioningStepAsync(tenantConnection, companyId, "RunTenantSchema", "Success", "Default schema applied.");

                var tenantScriptPath = Path.Combine(_env.ContentRootPath, "ProvisioningScripts", "DefaultSchema.sql");
                var tenantScript = await File.ReadAllTextAsync(tenantScriptPath);
                await new SqlCommand(tenantScript, tenantConnection).ExecuteNonQueryAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "SaveMetadata", "Success", "Company metadata saved.");

                // Step 4: Save metadata
                var company = new Company
                {
                    Id = companyId,
                    Name = companyName,
                    DbName = dbName,
                    AdminEmail = adminEmail,
                    BillingPlan = billingPlan,
                    ModulesEnabled = string.Join(",", modules),
                    CreatedAt = DateTime.UtcNow
                };

                await _metadataService.SaveAsync(company);
                await transaction.CommitAsync();

                _logger.LogInformation("Provisioned company '{CompanyName}' with DB '{DbName}'", companyName, dbName);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await LogProvisioningStepAsync(masterConnection, companyId, "Provisioning", "Failed", ex.Message);
                _logger.LogError(ex, "Provisioning failed for company '{CompanyName}'", companyName);
                return false;
            }
        }
    }
}