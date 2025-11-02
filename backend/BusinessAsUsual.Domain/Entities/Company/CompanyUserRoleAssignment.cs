using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.Company
{
    /// <summary>
    /// Represents the assignment of a user to a specific role within a company.
    /// </summary>
    public class CompanyUserRoleAssignment
    {
        /// <summary>
        /// Unique identifier for the role assignment entry.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "Assignment ID is required.")]
        public Guid AssignmentID { get; set; }

        /// <summary>
        /// Identifier of the user receiving the role.
        /// </summary>
        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserID { get; set; }

        /// <summary>
        /// Identifier of the role being assigned.
        /// </summary>
        [Required(ErrorMessage = "Role ID is required.")]
        public Guid RoleID { get; set; }

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
        /// The user receiving the role.
        /// </summary>
        public CompanyUser User { get; set; } = new CompanyUser();

        /// <summary>
        /// The role being assigned.
        /// </summary>
        public CompanyUserRole Role { get; set; } = new CompanyUserRole();
    }
}