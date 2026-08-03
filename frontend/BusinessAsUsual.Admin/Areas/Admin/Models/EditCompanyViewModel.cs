using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// View model for editing an existing company's details.
    /// </summary>
    public class EditCompanyViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the company being edited.
        /// </summary>
        [Required]
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the admin email address.
        /// </summary>
        [Required(ErrorMessage = "Admin email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the billing plan (Free, Professional, Enterprise).
        /// </summary>
        [Required(ErrorMessage = "Billing plan is required.")]
        public string BillingPlan { get; set; } = "Free";

        /// <summary>
        /// Gets or sets whether the company is currently archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of enabled modules.
        /// </summary>
        public string? ModulesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of enabled submodules.
        /// </summary>
        public string? SubmodulesEnabled { get; set; }

        /// <summary>
        /// Gets or sets additional notes about the company.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the list of available modules grouped by category.
        /// </summary>
        public List<ModuleGroupViewModel> GroupedModules { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of selected module IDs.
        /// </summary>
        public List<string> SelectedModules { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of selected submodule IDs.
        /// </summary>
        public List<string> SelectedSubmodules { get; set; } = new();

        /// <summary>
        /// Gets or sets the creation date (read-only for display).
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last modified date (read-only for display).
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }
    }
}
