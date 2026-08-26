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

        public Guid CompanyId => _tenantContext.CompanyId;
        public Guid? UserId => _tenantContext.UserId;
        public bool IsResolved => _tenantContext.IsResolved;
    }
}
