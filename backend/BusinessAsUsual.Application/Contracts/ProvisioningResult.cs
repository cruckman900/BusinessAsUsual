namespace BusinessAsUsual.Application.Contracts
{
    /// <summary>
    /// Represents the result of a company provisioning operation, including success status and related details.
    /// </summary>
    /// <remarks>Use this class to determine whether a provisioning request completed successfully and to
    /// access any associated error information or identifiers. The properties provide details about the outcome, such
    /// as the company identifier and tenant database name, when applicable.</remarks>
    public class ProvisioningResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message associated with the current operation.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated company.
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the database associated with the current tenant.
        /// </summary>
        public string? TenantDbName { get; set; }
    }
}
