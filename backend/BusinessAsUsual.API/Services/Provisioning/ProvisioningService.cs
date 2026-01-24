using BusinessAsUsual.API.Database;
using BusinessAsUsual.API.Common.DTOs;
using BusinessAsUsual.API.Services.Provisioning;
using BusinessAsUsual.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System.Text;

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

            var raw = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Missing AWS_SQL_CONNECTION_STRING");

            var builder = new SqlConnectionStringBuilder(raw)
            {
                InitialCatalog = "BusinessAsUsual" // ← reset to admin DB
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var command = conn.CreateCommand();
            command.CommandText = @"
            INSERT INTO ProvisioningLog (TenantName, Step, Status, Message, Timestamp)
            VALUES (@TenantName, @Step, @Status, @Message, GETUTCDATE())";

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
    /// Provides tenant provisioning operations, including orchestration of company creation, database setup, and
    /// initial configuration for new tenants.
    /// </summary>
    /// <remarks>This service coordinates the end-to-end provisioning workflow for new tenants, including
    /// database initialization and schema application. It is intended to be used by controllers or other application
    /// layers that require automated onboarding of companies into the system. The service is not thread-safe and should
    /// be scoped appropriately in dependency injection. For advanced provisioning scenarios, use the overload that
    /// accepts a ProvisioningRequest to specify additional options.</remarks>
    public class ProvisioningService : IProvisioningService
    {
        private readonly IProvisioningDb _db;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the ProvisioningService class with the specified database and hosting
        /// environment.
        /// </summary>
        /// <param name="db">The database context used for provisioning operations. Cannot be null.</param>
        /// <param name="env">The web hosting environment that provides information about the application's environment. Cannot be null.</param>
        public ProvisioningService(IProvisioningDb db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Legacy-compatible entry point used by CompanyController
        /// <summary>
        /// Provisions a new tenant with the specified company information, administrator email, billing plan, and
        /// enabled modules.
        /// </summary>
        /// <remarks>This method is provided for legacy compatibility and delegates to the main
        /// provisioning logic. For new development, consider using the overload that accepts a provisioning request
        /// object.</remarks>
        /// <param name="companyName">The name of the company for which the tenant is being provisioned. Cannot be null or empty.</param>
        /// <param name="adminEmail">The email address of the administrator for the new tenant. Must be a valid email address.</param>
        /// <param name="billingPlan">The billing plan to assign to the tenant. Must correspond to a supported plan.</param>
        /// <param name="modules">An array of module names to enable for the tenant. May be empty to provision with no modules enabled.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the tenant
        /// was provisioned successfully; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> ProvisionTenantAsync(
            string companyName,
            string adminEmail,
            string billingPlan,
            string[] modules)
        {
            var result = await ProvisionTenantAsync(new ProvisioningRequest
            {
                CompanyName = companyName,
                AdminEmail = adminEmail,
                BillingPlan = billingPlan,
                Modules = modules
            });

            return result.Success;
        }

        // New, richer orchestration entry point
        /// <summary>
        /// Provisions a new tenant by creating the necessary database structures and company records based on the
        /// specified provisioning request.
        /// </summary>
        /// <remarks>This method orchestrates the full tenant provisioning workflow, including master
        /// database validation, schema application, company record creation, and tenant database setup. Additional
        /// provisioning steps, such as module setup and onboarding, may be added in future versions. The method does
        /// not throw exceptions; any errors are captured in the returned ProvisioningResult.</remarks>
        /// <param name="request">The provisioning request containing company details, administrator information, billing plan, and enabled
        /// modules. Cannot be null.</param>
        /// <returns>A ProvisioningResult object that indicates whether the provisioning operation succeeded and provides details
        /// such as the company identifier and tenant database name.</returns>
        public async Task<ProvisioningResult> ProvisionTenantAsync(ProvisioningRequest request)
        {
            var result = new ProvisioningResult();

            try
            {
                // 1) Ensure master DB exists
                await _db.EnsureMasterDatabaseExistsAsync();

                // 2) Apply master schema (if needed)
                var masterSchemaPath = Path.Combine(
                    _env.ContentRootPath,
                    "ProvisioningScripts",
                    "MasterSchema.sql");

                if (File.Exists(masterSchemaPath))
                {
                    var masterScript = await File.ReadAllTextAsync(masterSchemaPath, Encoding.UTF8);
                    await _db.ApplyMasterSchemaAsync(masterScript);
                }

                // 3) Build company + tenant info
                var companyId = Guid.NewGuid();
                var tenantDbName = BuildTenantDbName(request.CompanyName);

                var company = new Company
                {
                    Id = companyId,
                    Name = request.CompanyName,
                    DbName = tenantDbName,
                    AdminEmail = request.AdminEmail,
                    BillingPlan = request.BillingPlan,
                    ModulesEnabled = string.Join(",", request.Modules ?? Array.Empty<string>()),
                    CreatedAt = DateTime.UtcNow
                };

                // 4) Save company record in master DB
                await _db.SaveCompanyInfoAsync(company);

                // 5) Create tenant DB
                await _db.CreateTenantDatabaseAsync(tenantDbName);

                // 6) Apply tenant schema
                var tenantSchemaPath = Path.Combine(
                    _env.ContentRootPath,
                    "ProvisioningScripts",
                    "DefaultSchema.sql");

                if (File.Exists(tenantSchemaPath))
                {
                    var tenantScript = await File.ReadAllTextAsync(tenantSchemaPath, Encoding.UTF8);
                    await _db.ApplyTenantSchemaAsync(tenantDbName, tenantScript);
                }

                // 7) TODO: Provision modules, onboarding token, admin user, audit log

                result.Success = true;
                result.CompanyId = companyId;
                result.TenantDbName = tenantDbName;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ProvisioningService.ProvisionTenantAsync error: {ex}");
                result.Success = false;
                result.Error = ex.Message;
                return result;
            }
        }

        private static string BuildTenantDbName(string companyName)
        {
            // Simple slug: strip non-alphanumerics, upper-case, prefix with BAU_
            var cleaned = new string(companyName
                .Where(char.IsLetterOrDigit)
                .ToArray());

            if (string.IsNullOrWhiteSpace(cleaned))
                cleaned = "TENANT";

            return $"BAU_{cleaned.ToUpperInvariant()}";
        }
    }
}