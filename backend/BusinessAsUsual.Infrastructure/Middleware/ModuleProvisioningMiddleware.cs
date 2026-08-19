using BusinessAsUsual.Application.Database;
using BusinessAsUsual.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BusinessAsUsual.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware that intercepts requests to module routes and provisions module schemas on first access (lazy-loading).
    /// This allows module-specific database tables to be created only when the module is actually used.
    /// </summary>
    public class ModuleProvisioningMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ModuleProvisioningMiddleware> _logger;

        public ModuleProvisioningMiddleware(
            RequestDelegate next,
            ILogger<ModuleProvisioningMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IProvisioningDb provisioningDb)
        {
            // Only process API routes that match module patterns
            var path = context.Request.Path.Value?.ToLowerInvariant();

            if (path != null && path.StartsWith("/api/"))
            {
                // Extract potential module identifier from path (e.g., /api/hr/employees -> "hr")
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 2)
                {
                    var potentialModuleKey = segments[1]; // "hr", "crm", "finance", etc.

                    // Get tenant context from headers or claims (placeholder for now)
                    var companyId = GetCompanyIdFromContext(context);
                    var tenantDbName = GetTenantDbNameFromContext(context);

                    if (companyId.HasValue && !string.IsNullOrEmpty(tenantDbName))
                    {
                        try
                        {
                            // Check if module needs provisioning
                            var moduleConfig = await provisioningDb.GetModuleConfigurationAsync(tenantDbName, companyId.Value);

                            if (!string.IsNullOrEmpty(moduleConfig))
                            {
                                var config = JsonSerializer.Deserialize<ModuleConfigurationRoot>(moduleConfig);

                                if (config != null)
                                {
                                    // Find the module by key
                                    var module = config.Modules.FirstOrDefault(m =>
                                        m.Group.Equals(potentialModuleKey, StringComparison.OrdinalIgnoreCase) ||
                                        m.ModuleName.Equals(potentialModuleKey, StringComparison.OrdinalIgnoreCase));

                                    if (module != null && module.Enabled && !module.IsProvisioned)
                                    {
                                        // Module needs to be provisioned
                                        _logger.LogInformation(
                                            "Module {ModuleName} accessed for the first time by tenant {TenantDbName}. Starting lazy provisioning...",
                                            module.ModuleName,
                                            tenantDbName);

                                        // TODO: Load and execute module-specific schema script
                                        // For now, just mark as provisioned
                                        module.IsProvisioned = true;
                                        config.LastUpdated = DateTime.UtcNow;

                                        var updatedConfig = JsonSerializer.Serialize(config);
                                        await provisioningDb.SaveModuleConfigurationToTenantAsync(
                                            tenantDbName,
                                            companyId.Value,
                                            updatedConfig);

                                        _logger.LogInformation(
                                            "Module {ModuleName} successfully provisioned for tenant {TenantDbName}",
                                            module.ModuleName,
                                            tenantDbName);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Error during lazy module provisioning for path {Path}, tenant {TenantDbName}",
                                path,
                                tenantDbName);
                            // Don't block the request - log error and continue
                        }
                    }
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Extracts the company/tenant ID from the current HTTP context.
        /// This is a placeholder - actual implementation would retrieve from JWT claims, headers, or session.
        /// </summary>
        private Guid? GetCompanyIdFromContext(HttpContext context)
        {
            // TODO: Implement actual tenant identification logic
            // Options:
            // 1. JWT claim: context.User.FindFirst("CompanyId")?.Value
            // 2. Custom header: context.Request.Headers["X-Company-Id"]
            // 3. Subdomain parsing: context.Request.Host.Host
            // 4. Session/cookie: context.Session.GetString("CompanyId")

            if (context.Request.Headers.TryGetValue("X-Company-Id", out var companyIdHeader))
            {
                if (Guid.TryParse(companyIdHeader, out var companyId))
                {
                    return companyId;
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts the tenant database name from the current HTTP context.
        /// This is a placeholder - actual implementation would retrieve from JWT claims, headers, or tenant resolution service.
        /// </summary>
        private string? GetTenantDbNameFromContext(HttpContext context)
        {
            // TODO: Implement actual tenant DB name resolution
            // Options:
            // 1. JWT claim: context.User.FindFirst("TenantDbName")?.Value
            // 2. Custom header: context.Request.Headers["X-Tenant-Db"]
            // 3. Lookup from CompanyId via master database
            // 4. Session/cookie: context.Session.GetString("TenantDbName")

            if (context.Request.Headers.TryGetValue("X-Tenant-Db", out var tenantDb))
            {
                return tenantDb.ToString();
            }

            return null;
        }
    }
}
