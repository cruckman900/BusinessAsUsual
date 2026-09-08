using BusinessAsUsual.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BusinessAsUsual.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware that resolves tenant context from JWT claims or request headers.
    /// Populates ITenantContext for downstream services and repositories.
    /// Requests targeting endpoints that are not marked with <see cref="AllowAnonymousTenantAttribute"/>
    /// must resolve a tenant context; otherwise the request is short-circuited with 401 Unauthorized.
    /// This must be registered after UseRouting() (or after MapControllers/endpoint resolution is
    /// available) so that endpoint metadata can be inspected.
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
            else if (!IsExempt(context))
            {
                // Tenant context is required for this endpoint and could not be resolved.
                // Fail fast here instead of letting downstream repositories/services throw
                // deep in the call stack with a confusing InvalidOperationException.
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Tenant context could not be resolved.",
                    detail = "Provide tenant identification via JWT claims (CompanyId, TenantDb) or the X-Company-Id/X-Tenant-Db headers."
                });
                return;
            }

            await _next(context);
        }

        /// <summary>
        /// Determines whether the current request's endpoint is exempt from mandatory
        /// tenant resolution, either because no endpoint has been matched yet (e.g. health
        /// checks, static files, OpenAPI/Swagger) or because it is explicitly marked with
        /// <see cref="AllowAnonymousTenantAttribute"/>.
        /// </summary>
        private static bool IsExempt(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            // No matched endpoint (e.g. health checks, static assets, OpenAPI docs) - allow through.
            if (endpoint is null)
                return true;

            return endpoint.Metadata.GetMetadata<AllowAnonymousTenantAttribute>() is not null;
        }
    }
}
