using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.ProvisionedCompany
{
    /// <summary>
    /// Represents a registered company within the system.
    /// </summary>
    public class CompanyInfo
    {
        /// <summary>
        /// Unique identifier for the company.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Company ID is required.")]
        public Guid CompanyID { get; set; }

        /// <summary>
        /// Display name of the company.
        /// </summary>
        [Required(ErrorMessage = "Company name is required.")]
        [MaxLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Optional description or tagline for the company.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the company is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the company was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the company was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// Users associated with this company.
        /// </summary>
        public ICollection<CompanyUser> Users { get; set; } = new List<CompanyUser>();

        /// <summary>
        /// Roles defined for this company.
        /// </summary>
        public ICollection<CompanyUserRole> Roles { get; set; } = new List<CompanyUserRole>();

        /// <summary>
        /// Settings associated with this company.
        /// </summary>
        public ICollection<CompanySetting> Settings { get; set; } = new List<CompanySetting>();
    }
}