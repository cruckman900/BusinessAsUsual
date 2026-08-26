namespace Platform.Application.Interfaces
{
    /// <summary>
    /// Provides tenant context information for the current user/session.
    /// Host applications should implement this to bridge to their tenant resolution strategy.
    /// </summary>
    public interface ITenantContextProvider
    {
        /// <summary>
        /// Gets the unique identifier of the current tenant/company.
        /// </summary>
        Guid CompanyId { get; }

        /// <summary>
        /// Gets the current user's unique identifier.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Gets a value indicating whether the tenant context has been resolved.
        /// </summary>
        bool IsResolved { get; }
    }
}
