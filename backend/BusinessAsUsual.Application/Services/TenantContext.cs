namespace BusinessAsUsual.Application.Services
{
    /// <summary>
    /// Scoped service that holds tenant context information for the current request.
    /// Populated by TenantResolutionMiddleware and consumed by repositories and services.
    /// </summary>
    public class TenantContext : ITenantContext
    {
        private readonly ITenantContextAccessor? _accessor;
        private Guid _companyId;
        private string _tenantDbName = string.Empty;
        private Guid? _userId;
        private bool _isResolved;

        /// <summary>
        /// Creates a new scoped tenant context. <paramref name="accessor"/> is optional so this
        /// type remains usable in hosts (or tests) that have not registered the singleton
        /// <see cref="ITenantContextAccessor"/>; when present, every <see cref="SetContext"/>
        /// call mirrors the resolved tenant into it so code running outside this scope (e.g.
        /// <see cref="IHttpClientFactory"/> message handlers) can still see the current tenant.
        /// </summary>
        public TenantContext(ITenantContextAccessor? accessor = null)
        {
            _accessor = accessor;
        }

        /// <inheritdoc/>
        public Guid CompanyId
        {
            get
            {
                if (!_isResolved)
                    throw new InvalidOperationException("Tenant context has not been resolved. Ensure TenantResolutionMiddleware is registered.");
                return _companyId;
            }
        }

        /// <inheritdoc/>
        public string TenantDbName
        {
            get
            {
                if (!_isResolved)
                    throw new InvalidOperationException("Tenant context has not been resolved. Ensure TenantResolutionMiddleware is registered.");
                return _tenantDbName;
            }
        }

        /// <inheritdoc/>
        public Guid? UserId => _userId;

        /// <inheritdoc/>
        public bool IsResolved => _isResolved;

        /// <inheritdoc/>
        public void SetContext(Guid companyId, string tenantDbName, Guid? userId = null)
        {
            // Intentionally allow re-resolution: Blazor Server circuits are long-lived and may
            // assign a default tenant at circuit-open (see TenantContextCircuitHandler) before
            // the user actually signs in and selects/confirms their real tenant (see
            // Login.razor), or a user may explicitly switch tenants mid-session. The most
            // recent call always wins.
            _companyId = companyId;
            _tenantDbName = tenantDbName ?? throw new ArgumentNullException(nameof(tenantDbName));
            _userId = userId;
            _isResolved = true;

            // Mirror into the ambient accessor (if registered) so code running outside this
            // scoped instance's DI scope - e.g. IHttpClientFactory message handlers such as
            // TenantPropagationHandler - can still read the current tenant safely. AsyncLocal
            // flows with the logical call chain, so this stays correct per Blazor circuit /
            // per HTTP request without needing to inject the Scoped ITenantContext itself.
            if (_accessor is not null)
            {
                _accessor.Current = new TenantSnapshot(_companyId, _tenantDbName, _userId);
            }
        }
    }
}
