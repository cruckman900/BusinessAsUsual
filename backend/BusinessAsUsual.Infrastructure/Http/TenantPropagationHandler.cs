using BusinessAsUsual.Application.Services;

namespace BusinessAsUsual.Infrastructure.Http
{
    /// <summary>
    /// A <see cref="DelegatingHandler"/> that automatically attaches the current
    /// <see cref="ITenantContext"/> (CompanyId/TenantDb/UserId) as request headers
    /// (<c>X-Company-Id</c>, <c>X-Tenant-Db</c>, <c>X-User-Id</c>) to every outgoing HTTP
    /// request made through a named/typed <see cref="HttpClient"/>.
    /// <para>
    /// This removes the need to hand-wire tenant headers at every call site that talks to a
    /// tenant-enforced downstream API (see <c>TenantResolutionMiddleware</c>), which is
    /// error-prone and easy to forget. Register via
    /// <c>builder.Services.AddTransient&lt;TenantPropagationHandler&gt;()</c> and attach with
    /// <c>.AddHttpMessageHandler&lt;TenantPropagationHandler&gt;()</c> on each named/typed
    /// HttpClient that calls a tenant-enforced downstream service.
    /// </para>
    /// <para>
    /// If the tenant context has not been resolved yet (e.g. a startup/seed-time call made
    /// before any user has signed in), no headers are added and the request proceeds
    /// unmodified - the downstream <c>TenantResolutionMiddleware</c> will reject it unless the
    /// target endpoint is exempt via <c>[AllowAnonymousTenant]</c>.
    /// </para>
    /// <para>
    /// <b>Important:</b> this handler intentionally depends on the singleton
    /// <see cref="ITenantContextAccessor"/> - NOT the Scoped <see cref="ITenantContext"/>
    /// directly. <see cref="IHttpClientFactory"/> constructs and caches
    /// <see cref="DelegatingHandler"/> instances (per handler lifetime, ~2 minutes by default)
    /// from its own internal factory-managed DI scope, which is unrelated to the scope of the
    /// code that calls <c>CreateClient</c>. Injecting the Scoped <see cref="ITenantContext"/>
    /// here would capture whichever tenant happened to be active the first time the handler
    /// was constructed and then silently keep reusing it for every subsequent circuit/request
    /// until the handler recycles - a cross-tenant data leak risk. The
    /// <see cref="ITenantContextAccessor"/> instead stores the tenant in an
    /// <see cref="AsyncLocal{T}"/>, which correctly flows with the logical call chain
    /// (including a Blazor Server circuit's captured <c>ExecutionContext</c>), so it always
    /// reflects the tenant of whichever circuit/request actually initiated the outgoing call.
    /// </para>
    /// </summary>
    public class TenantPropagationHandler : DelegatingHandler
    {
        private readonly ITenantContextAccessor _tenantContextAccessor;

        public TenantPropagationHandler(ITenantContextAccessor tenantContextAccessor)
        {
            _tenantContextAccessor = tenantContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tenant = _tenantContextAccessor.Current;
            if (tenant is not null)
            {
                request.Headers.Remove("X-Company-Id");
                request.Headers.Remove("X-Tenant-Db");
                request.Headers.Remove("X-User-Id");

                request.Headers.Add("X-Company-Id", tenant.CompanyId.ToString());
                request.Headers.Add("X-Tenant-Db", tenant.TenantDbName);

                if (tenant.UserId.HasValue)
                {
                    request.Headers.Add("X-User-Id", tenant.UserId.Value.ToString());
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
