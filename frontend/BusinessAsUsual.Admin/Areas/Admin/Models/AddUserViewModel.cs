using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Represents the data required to add a new admin portal user.
    /// </summary>
    public class AddUserViewModel
    {
        // ────────────────────────────────
        // Step 1: Basic Information
        // ────────────────────────────────

        /// <summary>
        /// Gets or sets the username for the admin user.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_\-\.]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, hyphens, and periods.")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address for the admin user.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the admin user.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password confirmation.
        /// </summary>
        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the admin user.
        /// </summary>
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the admin user.
        /// </summary>
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        // ────────────────────────────────
        // Step 2: Role Assignment
        // ────────────────────────────────

        /// <summary>
        /// Gets or sets the collection of role IDs assigned to this user.
        /// </summary>
        public List<string> SelectedRoles { get; set; } = new();

        /// <summary>
        /// Gets or sets the available roles for assignment (populated by controller).
        /// </summary>
        public List<AdminRoleOption> AvailableRoles { get; set; } = new();

        // ────────────────────────────────
        // Additional Properties
        // ────────────────────────────────

        /// <summary>
        /// Gets or sets whether the user should be active immediately.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets optional notes about the user.
        /// </summary>
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets the full display name of the user.
        /// </summary>
        public string DisplayName => 
            !string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName)
                ? $"{FirstName} {LastName}".Trim()
                : UserName;
    }

    /// <summary>
    /// Represents an admin role option for selection.
    /// </summary>
    public class AdminRoleOption
    {
        /// <summary>
        /// Gets or sets the unique identifier for the role.
        /// </summary>
        public string RoleId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the role.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of what this role allows.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this role is selected.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
