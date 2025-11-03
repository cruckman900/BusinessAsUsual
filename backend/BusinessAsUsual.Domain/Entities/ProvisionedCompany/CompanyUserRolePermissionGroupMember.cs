using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.ProvisionedCompany
{
    /// <summary>
    /// Represents a permission included in a specific permission group.
    /// </summary>
    public class CompanyUserRolePermissionGroupMember
    {
        /// <summary>
        /// Unique identifier for the group membership entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Group member ID is required.")]
        public Guid GroupMemberID { get; set; }

        /// <summary>
        /// Identifier of the permission group this member belongs to.
        /// </summary>
        [Required(ErrorMessage = "Permission group ID is required.")]
        public Guid PermissionGroupID { get; set; }

        /// <summary>
        /// Identifier of the permission included in the group.
        /// </summary>
        [Required(ErrorMessage = "Permission ID is required.")]
        public Guid PermissionID { get; set; }

        /// <summary>
        /// Indicates whether this membership is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the membership was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the membership was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The permission group this member belongs to.
        /// </summary>
        public CompanyUserRolePermissionGroup PermissionGroup { get; set; } = new CompanyUserRolePermissionGroup();

        /// <summary>
        /// The permission included in the group.
        /// </summary>
        public CompanyUserRolePermission Permission { get; set; } = new CompanyUserRolePermission();
    }
}