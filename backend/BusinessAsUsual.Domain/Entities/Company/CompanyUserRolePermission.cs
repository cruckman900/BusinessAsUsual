using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.Company
{
    /// <summary>
    /// Represents a specific permission assigned to a user role within a company.
    /// </summary>
    public class CompanyUserRolePermission
    {
        /// <summary>
        /// Unique identifier for the permission entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Permission ID is required.")]
        public Guid PermissionID { get; set; }

        /// <summary>
        /// Identifier of the role this permission is assigned to.
        /// </summary>
        [Required(ErrorMessage = "Role ID is required.")]
        public Guid RoleID { get; set; }

        /// <summary>
        /// Name or key of the permission (e.g., ViewReports, EditUsers).
        /// </summary>
        [Required(ErrorMessage = "Permission key is required.")]
        [MaxLength(100, ErrorMessage = "Permission key cannot exceed 100 characters.")]
        public string PermissionKey { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of what this permission allows.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the permission is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the permission was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the permission was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The role this permission is assigned to.
        /// </summary>
        public CompanyUserRole Role { get; set; } = new CompanyUserRole();

        /// <summary>
        /// Settings associated with this permission.
        /// </summary>
        public ICollection<CompanyUserRolePermissionSetting> Settings { get; set; } = new List<CompanyUserRolePermissionSetting>();

        /// <summary>
        /// Group memberships this permission belongs to.
        /// </summary>
        public ICollection<CompanyUserRolePermissionGroupMember> GroupMemberships { get; set; }

        /// <summary>
        /// Overrides applied to this permission.
        /// </summary>
        public ICollection<CompanyUserRolePermissionOverride> Overrides { get; set; }
    }
}