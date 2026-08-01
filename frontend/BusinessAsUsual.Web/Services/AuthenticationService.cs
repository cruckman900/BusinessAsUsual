namespace BusinessAsUsual.Web.Services;

/// <summary>
/// Service for managing user authentication state in the application.
/// </summary>
public class AuthenticationService
{
    private UserSession? _currentUser;

    /// <summary>
    /// Event raised when authentication state changes (login/logout).
    /// </summary>
    public event Action? OnAuthStateChanged;

    /// <summary>
    /// Gets the current authenticated user session.
    /// </summary>
    public UserSession? CurrentUser => _currentUser;

    /// <summary>
    /// Gets a value indicating whether a user is currently authenticated.
    /// </summary>
    public bool IsAuthenticated => _currentUser != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    public AuthenticationService()
    {
        // Initialize with a default test user for development
        // Remove this in production
        Login("admin", "admin@businessasusual.com", "Admin User", "Administrator");
    }

    /// <summary>
    /// Logs in a user with the specified credentials.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <param name="role">The user's role. Defaults to "User".</param>
    public void Login(string username, string email, string fullName, string role = "User")
    {
        _currentUser = new UserSession
        {
            Username = username,
            Email = email,
            FullName = fullName,
            Role = role,
            LoginTime = DateTime.UtcNow
        };
        OnAuthStateChanged?.Invoke();
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    public void Logout()
    {
        _currentUser = null;
        OnAuthStateChanged?.Invoke();
    }

    /// <summary>
    /// Updates the current user's profile information.
    /// </summary>
    /// <param name="fullName">The updated full name.</param>
    /// <param name="email">The updated email address.</param>
    /// <param name="phoneNumber">The updated phone number (optional).</param>
    /// <param name="jobTitle">The updated job title (optional).</param>
    /// <param name="department">The updated department (optional).</param>
    public void UpdateProfile(string fullName, string email, string? phoneNumber, string? jobTitle, string? department)
    {
        if (_currentUser != null)
        {
            _currentUser.FullName = fullName;
            _currentUser.Email = email;
            _currentUser.PhoneNumber = phoneNumber;
            _currentUser.JobTitle = jobTitle;
            _currentUser.Department = department;
            OnAuthStateChanged?.Invoke();
        }
    }

    /// <summary>
    /// Updates the current user's preferences.
    /// </summary>
    /// <param name="preferences">The updated user preferences.</param>
    public void UpdatePreferences(UserPreferences preferences)
    {
        if (_currentUser != null)
        {
            _currentUser.Preferences = preferences;
            OnAuthStateChanged?.Invoke();
        }
    }
}

/// <summary>
/// Represents an authenticated user session.
/// </summary>
public class UserSession
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's role.
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the user's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the user's job title.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the user's department.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the time when the user logged in.
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// Gets or sets the user's preferences.
    /// </summary>
    public UserPreferences Preferences { get; set; } = new();
}

/// <summary>
/// Represents user preferences for the application.
/// </summary>
public class UserPreferences
{
    /// <summary>
    /// Gets or sets the user's preferred language.
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the user's preferred time zone.
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// Gets or sets the user's preferred date format.
    /// </summary>
    public string DateFormat { get; set; } = "MM/dd/yyyy";

    /// <summary>
    /// Gets or sets the user's preferred time format.
    /// </summary>
    public string TimeFormat { get; set; } = "12h";

    /// <summary>
    /// Gets or sets a value indicating whether email notifications are enabled.
    /// </summary>
    public bool EmailNotifications { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether push notifications are enabled.
    /// </summary>
    public bool PushNotifications { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether dark mode is enabled.
    /// </summary>
    public bool DarkMode { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of items to display per page in lists.
    /// </summary>
    public int ItemsPerPage { get; set; } = 25;
}
