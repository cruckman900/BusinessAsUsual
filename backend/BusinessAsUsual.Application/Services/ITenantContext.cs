namespace BusinessAsUsual.Application.Services
{
    /// <summary>
    /// Provides tenant context information for the current request.
    /// Injected as a scoped service to provide tenant isolation across the application.
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// Gets the unique identifier of the current tenant/company.
        /// </summary>
        Guid CompanyId { get; }

        /// <summary>
        /// Gets the database name for the current tenant.
        /// </summary>
        string TenantDbName { get; }

        /// <summary>
        /// Gets the current user's unique identifier.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Gets a value indicating whether the tenant context has been resolved.
        /// </summary>
        bool IsResolved { get; }

        /// <summary>
        /// Sets the tenant context information.
        /// Called by TenantResolutionMiddleware after extracting tenant from request.
        /// </summary>
        void SetContext(Guid companyId, string tenantDbName, Guid? userId = null);
    }
}
