using BusinessAsUsual.Application.Database;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BusinessAsUsual.Infrastructure.Provisioning
{
    /// <summary>
    /// Service for discovering and executing module provisioning scripts.
    /// Module scripts are expected to be embedded resources in the format:
    /// {ModuleKey}ModuleSchema.sql (e.g., HRModuleSchema.sql)
    /// </summary>
    public interface IModuleScriptExecutor
    {
        Task ExecuteModuleScriptAsync(string tenantDbName, string moduleKey);
    }

    public class ModuleScriptExecutor : IModuleScriptExecutor
    {
        private readonly IProvisioningDb _provisioningDb;
        private readonly ILogger<ModuleScriptExecutor> _logger;
        private static readonly Dictionary<string, string> _moduleScriptMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // Map module keys to their embedded script resource paths
            // Format: ModuleKey -> AssemblyQualifiedName|ResourceName
            ["HR"] = "HR.Infrastructure|HR.Infrastructure.ProvisioningScripts.HRModuleSchema.sql",
            // Future modules can be added here:
            // ["Finance"] = "Finance.Infrastructure|Finance.Infrastructure.ProvisioningScripts.FinanceModuleSchema.sql",
            // ["CRM"] = "CRM.Infrastructure|CRM.Infrastructure.ProvisioningScripts.CRMModuleSchema.sql",
        };

        public ModuleScriptExecutor(
            IProvisioningDb provisioningDb,
            ILogger<ModuleScriptExecutor> logger)
        {
            _provisioningDb = provisioningDb;
            _logger = logger;
        }

        public async Task ExecuteModuleScriptAsync(string tenantDbName, string moduleKey)
        {
            if (!_moduleScriptMap.TryGetValue(moduleKey, out var scriptPath))
            {
                _logger.LogWarning(
                    "No provisioning script found for module {ModuleKey}. Module may not have schema or is not yet registered.",
                    moduleKey);
                return;
            }

            var parts = scriptPath.Split('|');
            if (parts.Length != 2)
            {
                _logger.LogError("Invalid script path format for module {ModuleKey}: {ScriptPath}", moduleKey, scriptPath);
                return;
            }

            var assemblyName = parts[0];
            var resourceName = parts[1];

            try
            {
                // Load the module assembly
                var assembly = Assembly.Load(assemblyName);

                // Read the embedded resource
                using var stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    _logger.LogError(
                        "Embedded resource {ResourceName} not found in assembly {AssemblyName} for module {ModuleKey}",
                        resourceName,
                        assemblyName,
                        moduleKey);
                    return;
                }

                using var reader = new StreamReader(stream);
                var scriptContent = await reader.ReadToEndAsync();

                _logger.LogInformation(
                    "Executing module provisioning script for {ModuleKey} on tenant {TenantDbName}",
                    moduleKey,
                    tenantDbName);

                // Execute the script against the tenant database
                await _provisioningDb.ExecuteScriptAsync(tenantDbName, scriptContent);

                _logger.LogInformation(
                    "Successfully executed module provisioning script for {ModuleKey} on tenant {TenantDbName}",
                    moduleKey,
                    tenantDbName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to execute module provisioning script for {ModuleKey} on tenant {TenantDbName}",
                    moduleKey,
                    tenantDbName);
                throw;
            }
        }
    }
}
