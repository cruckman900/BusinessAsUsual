using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.Company
{
    /// <summary>
    /// Represents an audit log entry for changes made to a permission group.
    /// </summary>
    public class CompanyUserRolePermissionGroupAudit
    {
        /// <summary>
        /// Unique identifier for the audit entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Audit ID is required.")]
        public Guid AuditID { get; set; }

        /// <summary>
        /// Identifier of the permission group being audited.
        /// </summary>
        [Required(ErrorMessage = "Permission group ID is required.")]
        public Guid PermissionGroupID { get; set; }

        /// <summary>
        /// Description of the change made (e.g., Added permission X, Removed permission Y).
        /// </summary>
        [Required(ErrorMessage = "Change description is required.")]
        public string ChangeDescription { get; set; } = string.Empty;

        /// <summary>
        /// Identifier of the user who made the change.
        /// </summary>
        public Guid? ChangedByUserID { get; set; }

        /// <summary>
        /// Timestamp when the change occurred.
        /// </summary>
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The permission group this audit entry is associated with.
        /// </summary>
        public CompanyUserRolePermissionGroup PermissionGroup { get; set; } = new CompanyUserRolePermissionGroup();

        /// <summary>
        /// The user who made the change.
        /// </summary>
        public CompanyUser ChangedByUser { get; set; } = new CompanyUser();
    }
}