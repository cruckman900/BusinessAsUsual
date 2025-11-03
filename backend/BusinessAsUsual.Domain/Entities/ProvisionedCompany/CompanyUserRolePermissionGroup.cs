using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.ProvisionedCompany
{
    /// <summary>
    /// Represents a group of permissions that can be assigned to roles for modular access control.
    /// </summary>
    public class CompanyUserRolePermissionGroup
    {
        /// <summary>
        /// Unique identifier for the permission group.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Permission group ID is required.")]
        public Guid PermissionGroupID { get; set; }

        /// <summary>
        /// Identifier of the company this group belongs to.
        /// </summary>
        [Required(ErrorMessage = "Company ID is required.")]
        public Guid CompanyID { get; set; }

        /// <summary>
        /// Name of the permission group.
        /// </summary>
        [Required(ErrorMessage = "Group name is required.")]
        [MaxLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the group’s purpose or contents.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the group is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the group was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the group was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The company this group belongs to.
        /// </summary>
        public CompanyInfo Company { get; set; } = new CompanyInfo();

        /// <summary>
        /// Permissions included in this group.
        /// </summary>
        public ICollection<CompanyUserRolePermissionGroupMember> Members { get; set; } = new List<CompanyUserRolePermissionGroupMember>();

        /// <summary>
        /// Roles this group is assigned to.
        /// </summary>
        public ICollection<CompanyUserRolePermissionGroupAssignment> RoleAssignments { get; set; } = new List<CompanyUserRolePermissionGroupAssignment>();

        /// <summary>
        /// Audit history of changes to this group.
        /// </summary>
        public ICollection<CompanyUserRolePermissionGroupAudit> AuditTrail { get; set; } = new List<CompanyUserRolePermissionGroupAudit>();
    }
}