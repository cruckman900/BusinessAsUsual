using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.Company
{
    /// <summary>
    /// Represents a customizable setting associated with a specific role permission.
    /// </summary>
    public class CompanyUserRolePermissionSetting
    {
        /// <summary>
        /// Unique identifier for the permission setting entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Setting ID is required.")]
        public Guid SettingID { get; set; }

        /// <summary>
        /// Identifier of the permission this setting belongs to.
        /// </summary>
        [Required(ErrorMessage = "Permission ID is required.")]
        public Guid PermissionID { get; set; }

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
        /// The permission this setting belongs to.
        /// </summary>
        public CompanyUserRolePermission Permission { get; set; } = new CompanyUserRolePermission();
    }
}