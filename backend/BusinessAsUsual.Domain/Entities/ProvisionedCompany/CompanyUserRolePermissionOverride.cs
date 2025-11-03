using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.ProvisionedCompany
{
    /// <summary>
    /// Represents a permission override for a specific user, allowing exceptions to role-based permissions.
    /// </summary>
    public class CompanyUserRolePermissionOverride
    {
        /// <summary>
        /// Unique identifier for the override entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Override ID is required.")]
        public Guid OverrideID { get; set; }

        /// <summary>
        /// Identifier of the user receiving the override.
        /// </summary>
        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserID { get; set; }

        /// <summary>
        /// Identifier of the permission being overridden.
        /// </summary>
        [Required(ErrorMessage = "Permission ID is required.")]
        public Guid PermissionID { get; set; }

        /// <summary>
        /// Indicates whether the permission is granted or revoked.
        /// </summary>
        public bool IsGranted { get; set; }

        /// <summary>
        /// Optional notes or justification for the override.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the override was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the override was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The user receiving the override.
        /// </summary>
        public CompanyUser User { get; set; } = new CompanyUser();

        /// <summary>
        /// The permission being overridden.
        /// </summary>
        public CompanyUserRolePermission Permission { get; set; } = new CompanyUserRolePermission();
    }
}