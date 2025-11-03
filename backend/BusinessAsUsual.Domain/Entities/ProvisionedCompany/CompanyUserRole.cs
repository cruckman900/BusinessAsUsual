using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.ProvisionedCompany
{
    /// <summary>
    /// Represents a role defined within a company for access control and permissions.
    /// </summary>
    public class CompanyUserRole
    {
        /// <summary>
        /// Unique identifier for the role.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Role ID is required.")]
        public Guid RoleID { get; set; }

        /// <summary>
        /// Identifier of the company this role belongs to.
        /// </summary>
        [Required(ErrorMessage = "Company ID is required.")]
        public Guid CompanyID { get; set; }

        /// <summary>
        /// Name of the role (e.g., Admin, Manager, Viewer).
        /// </summary>
        [Required(ErrorMessage = "Role name is required.")]
        [MaxLength(100, ErrorMessage = "Role name cannot exceed 100 characters.")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the role's purpose.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the role is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the role was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the role was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The company this role belongs to.
        /// </summary>
        public CompanyInfo Company { get; set; } = new CompanyInfo();

        /// <summary>
        /// Settings associated with this role.
        /// </summary>
        public ICollection<CompanyUserRoleSetting> Settings { get; set; } = new List<CompanyUserRoleSetting>();

        /// <summary>
        /// Permissions assigned to this role.
        /// </summary>
        public ICollection<CompanyUserRolePermission> Permissions { get; set; } = new List<CompanyUserRolePermission>();

        /// <summary>
        /// Users assigned to this role.
        /// </summary>
        public ICollection<CompanyUserRoleAssignment> AssignedUsers { get; set; } = new List<CompanyUserRoleAssignment>();

        /// <summary>
        /// Permission group assignments for this role.
        /// </summary>
        public ICollection<CompanyUserRolePermissionGroupAssignment> PermissionGroupAssignments { get; set; } = new List<CompanyUserRolePermissionGroupAssignment>();
    }
}