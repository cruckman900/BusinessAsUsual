using System.ComponentModel.DataAnnotations;

namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// View model for admin login.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the return URL after successful login.
        /// </summary>
        public string? ReturnUrl { get; set; }
    }

    /// <summary>
    /// Result of a login attempt.
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// Gets or sets whether the login was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message if login failed.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the authenticated username.
        /// </summary>
        public string? Username { get; set; }
    }

    /// <summary>
    /// Admin dashboard insights.
    /// </summary>
    public class AdminInsightsViewModel
    {
        /// <summary>
        /// Gets or sets the total number of companies.
        /// </summary>
        public int TotalCompanies { get; set; }

        /// <summary>
        /// Gets or sets the number of active companies.
        /// </summary>
        public int ActiveCompanies { get; set; }

        /// <summary>
        /// Gets or sets the total number of admin users.
        /// </summary>
        public int TotalAdminUsers { get; set; }

        /// <summary>
        /// Gets or sets the system health status.
        /// </summary>
        public string SystemStatus { get; set; } = "Healthy";

        /// <summary>
        /// Gets or sets the system uptime in hours.
        /// </summary>
        public int UptimeHours { get; set; }

        /// <summary>
        /// Gets or sets whether this data came from mock/fallback.
        /// </summary>
        public bool IsMockData { get; set; }
    }
}
