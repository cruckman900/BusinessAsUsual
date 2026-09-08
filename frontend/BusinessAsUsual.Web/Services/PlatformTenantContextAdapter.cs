using Platform.Application.Interfaces;

namespace BusinessAsUsual.Web.Services
{
    /// <summary>
    /// Adapter that bridges BusinessAsUsual.Application.Services.ITenantContext
    /// to Platform.Application.Interfaces.ITenantContextProvider for use in Platform module components.
    /// </summary>
    public class PlatformTenantContextAdapter : ITenantContextProvider
    {
        private readonly Application.Services.ITenantContext _tenantContext;

        public PlatformTenantContextAdapter(Application.Services.ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        // Guarded against the underlying ITenantContext throwing InvalidOperationException:
        // Blazor Server prerenders components on the initial HTTP request, before the SignalR
        // circuit opens and TenantContextCircuitHandler assigns a default tenant. Any component
        // that reads CompanyId/UserId during prerender would otherwise crash the page. Returning
        // safe defaults here lets prerendered markup fall back gracefully; the real values are
        // available once the circuit is established and the interactive render kicks in.
        public Guid CompanyId => _tenantContext.IsResolved ? _tenantContext.CompanyId : Guid.Empty;
        public Guid? UserId => _tenantContext.IsResolved ? _tenantContext.UserId : null;
        public bool IsResolved => _tenantContext.IsResolved;
    }
}
