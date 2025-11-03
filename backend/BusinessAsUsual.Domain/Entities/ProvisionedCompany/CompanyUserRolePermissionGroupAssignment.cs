using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.ProvisionedCompany
{
    /// <summary>
    /// Represents the assignment of a permission group to a specific user role.
    /// </summary>
    public class CompanyUserRolePermissionGroupAssignment
    {
        /// <summary>
        /// Unique identifier for the group assignment entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Assignment ID is required.")]
        public Guid AssignmentID { get; set; }

        /// <summary>
        /// Identifier of the role receiving the permission group.
        /// </summary>
        [Required(ErrorMessage = "Role ID is required.")]
        public Guid RoleID { get; set; }

        /// <summary>
        /// Identifier of the permission group being assigned.
        /// </summary>
        [Required(ErrorMessage = "Permission group ID is required.")]
        public Guid PermissionGroupID { get; set; }

        /// <summary>
        /// Indicates whether this assignment is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the assignment was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the assignment was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The role receiving the permission group.
        /// </summary>
        public CompanyUserRole Role { get; set; } = new CompanyUserRole();

        /// <summary>
        /// The permission group being assigned.
        /// </summary>
        public CompanyUserRolePermissionGroup PermissionGroup { get; set; } = new CompanyUserRolePermissionGroup();
    }
}