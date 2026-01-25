using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities
{
    /// <summary>
    /// Represents the metadata related to a company within the Business As Usual system.
    /// </summary>
    public class CompanySettings
    {
        /// <summary>Unique identifier for the company settings record</summary>
        public Guid Id { get; set; }

        /// <summary>Company Id</summary>
        [Display(Name = "Company Id")]
        [Required(ErrorMessage = "Company Id is required.")]
        public Guid CompanyId { get; set; }

        /// <summary>Type of plan for services</summary>
        [Display(Name = "Billing Plan")]
        [Required(ErrorMessage = "Billing Plan is required.")]
        [MaxLength(50, ErrorMessage = "Billing Plan must be under 50 characters.")]
        public string BillingPlan { get; set; } = string.Empty;

        /// <summary>Services required by company</summary>
        [Display(Name = "Modules Enabled")]
        [Required(ErrorMessage = "Modules Enabled is required.")]
        public string ModulesEnabled { get; set; } = string.Empty;

        /// <summary>Toggle company access status</summary>
        [Display(Name = "Is Disabled?")]
        [Required(ErrorMessage = "Is Disabled? is required.")]
        public bool IsDisabled { get; set; } = false;

        /// <summary>
        /// UTC timestamp when the company was created.
        /// </summary>
        [Display(Name = "Created At")]
        [Required(ErrorMessage = "Created At is required.")]
        public DateTime CreatedAt { get; set; }
    }
}
