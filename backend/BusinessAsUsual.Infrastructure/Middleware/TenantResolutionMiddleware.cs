using BusinessAsUsual.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BusinessAsUsual.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware that resolves tenant context from JWT claims or request headers.
    /// Populates ITenantContext for downstream services and repositories.
    /// </summary>
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
        {
            // Priority 1: Extract from JWT claims (when authentication is implemented)
            var companyIdFromClaims = context.User?.FindFirst("CompanyId")?.Value;
            var tenantDbFromClaims = context.User?.FindFirst("TenantDb")?.Value;
            var userIdFromClaims = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Priority 2: Fall back to headers (for API testing, provisioning, admin operations)
            var companyIdFromHeader = context.Request.Headers["X-Company-Id"].FirstOrDefault();
            var tenantDbFromHeader = context.Request.Headers["X-Tenant-Db"].FirstOrDefault();
            var userIdFromHeader = context.Request.Headers["X-User-Id"].FirstOrDefault();

            // Resolve final values
            var companyIdString = companyIdFromClaims ?? companyIdFromHeader;
            var tenantDbName = tenantDbFromClaims ?? tenantDbFromHeader;
            var userIdString = userIdFromClaims ?? userIdFromHeader;

            // Validate and parse
            if (!string.IsNullOrEmpty(companyIdString) && Guid.TryParse(companyIdString, out var companyId)
                && !string.IsNullOrEmpty(tenantDbName))
            {
                Guid? userId = null;
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                tenantContext.SetContext(companyId, tenantDbName, userId);
            }
            else
            {
                // Allow requests without tenant context (health checks, swagger, etc.)
                // Downstream services will throw if they try to access unresolved context
            }

            await _next(context);
        }
    }
}
