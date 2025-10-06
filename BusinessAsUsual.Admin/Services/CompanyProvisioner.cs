using Microsoft.Data.SqlClient;
using BusinessAsUsual.Admin.Models;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Handles the provisioning of new company databases and schema setup.
    /// </summary>
    public class CompanyProvisioner
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;
        private readonly TenantMetadataService _metadataService;
        private readonly ILogger<CompanyProvisioner> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyProvisioner"/> class.
        /// </summary>
        /// <param name="config">Application configuration for connection strings.</param>
        /// <param name="env">Hosting environment for locating script files.</param>
        /// <param name="metadataService">Service for saving company metadata.</param>
        /// <param name="logger">Logging service</param>
        public CompanyProvisioner
            (
                IConfiguration config, 
                IHostEnvironment env,
                TenantMetadataService metadataService,
                ILogger<CompanyProvisioner> logger
            )
        {
            _config = config;
            _env = env;
            _metadataService = metadataService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new database for the specified company, seeds it with default schema,
        /// and logs metadata to the master database.
        /// </summary>
        /// <param name="companyName">The name of the company to provision.</param>
        /// <param name="adminEmail">The email address of the company admin.</param>
        /// <returns>True if provisioning succeeds; otherwise, false.</returns>
        public async Task<bool> CreateCompanyDatabaseAsync(string companyName, string adminEmail)
        {
            _logger.LogInformation("Starting provisioning for company: {CompanyName}", companyName);

            var dbName = $"bau_{companyName.ToLower().Replace(" ", "_")}";
            var masterConnStr = _config.GetConnectionString("MasterAdmin");

            await using var masterConnection = new SqlConnection(masterConnStr);
            await masterConnection.OpenAsync();

            if (masterConnection is null)
                throw new InvalidOperationException("Failed to create SQL connection.");

            await using var transaction = await masterConnection!.BeginTransactionAsync();

            if (transaction is null)
                throw new InvalidOperationException("Failed to begin SQL transaction.");

            try
            {
                var createDbCommand = $"CREATE DATABASE [{dbName}]";

                _logger.LogInformation("Creating database: {DbName}", dbName);
                await new SqlCommand(createDbCommand, masterConnection, (SqlTransaction)transaction!).ExecuteNonQueryAsync();

                _logger.LogInformation("Switching to database: {DbName}", dbName);
                await masterConnection.ChangeDatabaseAsync(dbName);

                var scriptPath = Path.Combine(_env.ContentRootPath, "ProvisioningScripts", "DefaultSchema.sql");
                var script = await File.ReadAllTextAsync(scriptPath);

                _logger.LogInformation("Executing provisioning script from: {ScriptPath}", scriptPath);
                await new SqlCommand(script, masterConnection, (SqlTransaction)transaction!).ExecuteNonQueryAsync();

                var company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = companyName,
                    DbName = dbName,
                    AdminEmail = adminEmail,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Saving metadata for company: {CompanyName}", companyName);
                await _metadataService.SaveAsync(company);

                await transaction.CommitAsync();

                _logger.LogInformation("Provisioning complete for company: {CompanyName}", companyName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provisioning failed for company: {CompanyName}", companyName);
                await transaction.RollbackAsync();
                return false;
            }
            finally
            {
                await transaction.DisposeAsync();
                await masterConnection.DisposeAsync();
            }
        }
    }
}