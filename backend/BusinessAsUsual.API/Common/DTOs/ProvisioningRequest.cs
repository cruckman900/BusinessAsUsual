namespace BusinessAsUsual.API.Common.DTOs
{
    /// <summary>
    /// Represents a request to provision a new company account, including administrative and billing details.
    /// </summary>
    /// <remarks>Use this class to supply the necessary information when initiating a company provisioning
    /// workflow, such as the company name, administrator contact, billing plan, and requested modules. All properties
    /// must be set before submitting the request. This type is typically used as a data transfer object in onboarding
    /// or account creation scenarios.</remarks>
    public class ProvisioningRequest
    {
        /// <summary>
        /// Gets or sets the name of the company associated with this instance.
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the administrator's email address used for system notifications and contact purposes.
        /// </summary>
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the billing plan associated with the account.
        /// </summary>
        public string BillingPlan { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of module names associated with the current instance.
        /// </summary>
        public string[] Modules { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the names of the submodules associated with this instance.
        /// </summary>
        public string[] Submodules { get; set; } = Array.Empty<string>();

        // Future: BusinessType, Submodules, OnboardingMethod, Notes, etc.
    }
}
