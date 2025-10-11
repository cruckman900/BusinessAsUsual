namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Data transer object representing a company provisioning request.
    /// </summary>
    public class ProvisioningRequest
    {
        /// <summary>
        /// Company Name
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Administrator Email
        /// </summary>
        public string AdminEmail { get; set;} = string.Empty;

        /// <summary>
        /// Billing Plan
        /// </summary>
        public string BillingPlan { get; set;} = string.Empty;

        /// <summary>
        /// Business Modules the tenant plans to use.
        /// </summary>
        public string[] Modules { get; set; } = Array.Empty<string>();
    }
}
