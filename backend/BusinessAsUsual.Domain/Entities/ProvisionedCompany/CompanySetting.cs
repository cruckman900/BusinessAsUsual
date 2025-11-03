using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.ProvisionedCompany
{
    /// <summary>
    /// Represents a customizable setting at the company level.
    /// </summary>
    public class CompanySetting
    {
        /// <summary>
        /// Unique identifier for the setting entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Setting ID is required.")]
        public Guid SettingID { get; set; }

        /// <summary>
        /// Identifier of the company this setting belongs to.
        /// </summary>
        [Required(ErrorMessage = "Company ID is required.")]
        public Guid CompanyID { get; set; }

        /// <summary>
        /// Key name of the setting.
        /// </summary>
        [Required(ErrorMessage = "Setting key is required.")]
        [MaxLength(100, ErrorMessage = "Setting key cannot exceed 100 characters.")]
        public string SettingKey { get; set; } = string.Empty;

        /// <summary>
        /// Value of the setting.
        /// </summary>
        public string SettingValue { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the setting is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the setting was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the setting was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The company this setting belongs to.
        /// </summary>
        public CompanyInfo Company { get; set; } = new CompanyInfo();
    }
}