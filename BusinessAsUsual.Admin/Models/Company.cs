using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Represents a company being provisioned into the system.
    /// </summary>
    public class Company
    {
        /// <summary>Unique identifier for the company.</summary>
        public Guid Id { get; set; }

        /// <summary>Name of the company.</summary>
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(100, ErrorMessage = "Company Name must be under 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Database name assigned to the company.</summary>
        [Display(Name = "Database Name")]
        public string DbName { get; set; } = string.Empty;

        /// <summary>Email address of the company admin.</summary>
        [Display(Name = "Admin Email")]
        [Required(ErrorMessage = "Admin Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Billing plan assigned to the company.
        /// </summary>
        [Display(Name = "Billing Plan")]
        [Required(ErrorMessage = "Billing Plan is required.")]
        public string BillingPlan { get; set; } = "Standard";

        /// <summary>
        /// Comma-separated list of enabled modules.
        /// </summary>
        [Display(Name = "Modules Enabled")]
        [Required(ErrorMessage = "Modules Enabled is required.")]
        public string ModulesEnabled { get; set; } = "Employees,Products";

        /// <summary>
        /// UTC timestamp when the company was created.
        /// </summary>
        [Display(Name = "Created At")]
        [Required(ErrorMessage = "Created At is required.")]
        public DateTime CreatedAt { get; set; }
    }
}