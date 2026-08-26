namespace BusinessAsUsual.Application.Services
{
    /// <summary>
    /// Scoped service that holds tenant context information for the current request.
    /// Populated by TenantResolutionMiddleware and consumed by repositories and services.
    /// </summary>
    public class TenantContext : ITenantContext
    {
        private Guid _companyId;
        private string _tenantDbName = string.Empty;
        private Guid? _userId;
        private bool _isResolved;

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
            if (_isResolved)
                throw new InvalidOperationException("Tenant context has already been set for this request.");

            _companyId = companyId;
            _tenantDbName = tenantDbName ?? throw new ArgumentNullException(nameof(tenantDbName));
            _userId = userId;
            _isResolved = true;
        }
    }
}
