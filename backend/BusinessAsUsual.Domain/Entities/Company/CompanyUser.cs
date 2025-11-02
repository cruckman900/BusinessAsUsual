using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Domain.Entities.Company
{
    /// <summary>
    /// Represents a user associated with a specific company.
    /// </summary>
    public class CompanyUser
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserID { get; set; }

        /// <summary>
        /// Identifier of the company this user belongs to.
        /// </summary>
        [Required(ErrorMessage = "Company ID is required.")]
        public Guid CompanyID { get; set; }

        /// <summary>
        /// User's display name or full name.
        /// </summary>
        [Required(ErrorMessage = "User name is required.")]
        [MaxLength(100, ErrorMessage = "User name cannot exceed 100 characters.")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the user was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ────────────────────────────────
        // Navigation Properties
        // ────────────────────────────────

        /// <summary>
        /// The company this user belongs to.
        /// </summary>
        public CompanyInfo Company { get; set; } = new CompanyInfo();

        /// <summary>
        /// Settings associated with this user.
        /// </summary>
        public ICollection<CompanyUserSetting> Settings { get; set; } = new List<CompanyUserSetting>();

        /// <summary>
        /// Roles assigned to this user.
        /// </summary>
        public ICollection<CompanyUserRoleAssignment> RoleAssignments { get; set; } = new List<CompanyUserRoleAssignment>();

        /// <summary>
        /// Permission overrides for this user.
        /// </summary>
        public ICollection<CompanyUserRolePermissionOverride> PermissionOverrides { get; set; } = new List<CompanyUserRolePermissionOverride>();
    }
}