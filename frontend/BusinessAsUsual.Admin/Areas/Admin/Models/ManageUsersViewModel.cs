namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// View model for the Manage Users page.
    /// </summary>
    public class ManageUsersViewModel
    {
        /// <summary>
        /// Gets or sets the list of users to display.
        /// </summary>
        public List<UserListItemViewModel> Users { get; set; } = new();

        /// <summary>
        /// Gets or sets the search query for filtering users.
        /// </summary>
        public string? SearchQuery { get; set; }

        /// <summary>
        /// Gets or sets the role filter.
        /// </summary>
        public string? RoleFilter { get; set; }

        /// <summary>
        /// Gets or sets the status filter (Active, Inactive, All).
        /// </summary>
        public string? StatusFilter { get; set; }

        /// <summary>
        /// Gets or sets the start date for filtering by creation date.
        /// </summary>
        public DateTime? CreatedFromDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for filtering by creation date.
        /// </summary>
        public DateTime? CreatedToDate { get; set; }

        /// <summary>
        /// Gets or sets the current page number for pagination.
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets the total number of users (before pagination).
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalUsers / PageSize);

        /// <summary>
        /// Gets or sets available roles for filtering.
        /// </summary>
        public List<string> AvailableRoles { get; set; } = new();
    }

    /// <summary>
    /// Represents a single user in the user list.
    /// </summary>
    public class UserListItemViewModel
    {
        /// <summary>
        /// Gets or sets the user's unique identifier.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets the full display name.
        /// </summary>
        public string DisplayName =>
            !string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName)
                ? $"{FirstName} {LastName}".Trim()
                : UserName;

        /// <summary>
        /// Gets or sets whether the user is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of role names.
        /// </summary>
        public string Roles { get; set; } = string.Empty;

        /// <summary>
        /// Gets the list of role names.
        /// </summary>
        public List<string> RoleList => Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .ToList();

        /// <summary>
        /// Gets or sets the date the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last login date (optional).
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Gets or sets optional notes about the user.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets the friendly created date display.
        /// </summary>
        public string CreatedAtDisplay => CreatedAt.ToString("MMM dd, yyyy");

        /// <summary>
        /// Gets the friendly last login display.
        /// </summary>
        public string LastLoginDisplay => LastLoginAt.HasValue
            ? LastLoginAt.Value.ToString("MMM dd, yyyy HH:mm")
            : "Never";
    }
}
