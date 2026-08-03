using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// View model for the View Companies page, containing a list of companies with filtering and display properties.
    /// </summary>
    public class ViewCompaniesViewModel
    {
        /// <summary>
        /// Gets or sets the list of companies to display.
        /// </summary>
        public List<CompanyListItemViewModel> Companies { get; set; } = new();

        /// <summary>
        /// Gets or sets the search query for filtering companies by name or ID.
        /// </summary>
        public string? SearchQuery { get; set; }

        /// <summary>
        /// Gets or sets the filter for company status (All, Active, Archived).
        /// </summary>
        public string StatusFilter { get; set; } = "Active";

        /// <summary>
        /// Gets or sets the filter for billing plan (All, Free, Professional, Enterprise).
        /// </summary>
        public string? BillingPlanFilter { get; set; }

        /// <summary>
        /// Gets or sets the total number of companies (before pagination, if applicable).
        /// </summary>
        public int TotalCompanies { get; set; }
    }

    /// <summary>
    /// Represents a single company in the company list view.
    /// </summary>
    public class CompanyListItemViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the company.
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the admin email address.
        /// </summary>
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the billing plan (e.g., Free, Professional, Enterprise).
        /// </summary>
        public string BillingPlan { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date the company was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets whether the company is currently archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of enabled modules.
        /// </summary>
        public string ModulesEnabled { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of users associated with this company (optional).
        /// </summary>
        public int? UserCount { get; set; }

        /// <summary>
        /// Gets a formatted display string for the creation date.
        /// </summary>
        public string CreatedAtDisplay => CreatedAt.ToString("MMM dd, yyyy");

        /// <summary>
        /// Gets a formatted display string for the status.
        /// </summary>
        public string StatusDisplay => IsArchived ? "Archived" : "Active";

        /// <summary>
        /// Gets a list of module names split from the ModulesEnabled string.
        /// </summary>
        public List<string> ModuleList => 
            ModulesEnabled.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(m => m.Trim())
                         .ToList();
    }
}
