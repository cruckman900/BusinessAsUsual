namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    ///     Defines the contract for provisioning new tenants within the Business As Usual platform.
    ///     This service encapsulates the orchestration logic required to initialize company data,
    ///     ssign administrative access, configure billing, and activate selected modules.
    /// </summary>
    public interface IProvisioningService
    {
        /// <summary>
        /// Provisions a new tenant by creating the necessary schema, assigning an administrator,
        /// configuring the billing plan, and enabling selected modules.
        /// </summary>
        /// <param name="companyName">
        ///     The name of the company to be provisioned. This will be used to generate schema identifiers and branding metadata.
        /// </param>
        /// <param name="adminEmail">
        ///     The email address of the primary administrator for the new tenant. This account will be granted initial access and elevated permissions.
        /// </param>
        /// <param name="billingPlan">
        ///     The billing tier selected for the tenant (e.g., Free, Standard, Enterprise). Determines feature access and resource limits.
        /// </param>
        /// <param name="modules">
        ///     An array of module identifiers to activate for the tenant (e.g., "Inventory", "Scheduling", "Billing"). Each module will be initialized and linked to the tenant's schema.
        /// </param>
        /// <returns>
        ///     A task that resolves to <c>true</c> if provisioning succeeds, or <c>false</c> if any step fails. Errors should be logged and surfaced via contributor-facing diagnostics.
        /// </returns>
        Task<bool> ProvisionTenantAsync(
            string companyName,
            string adminEmail,
            string billingPlan,
            string[] modules
        );
    }
}