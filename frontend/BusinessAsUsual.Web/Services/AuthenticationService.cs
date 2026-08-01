namespace BusinessAsUsual.Web.Services;

public class AuthenticationService
{
    private UserSession? _currentUser;
    public event Action? OnAuthStateChanged;

    public UserSession? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public AuthenticationService()
    {
        // Initialize with a default test user for development
        // Remove this in production
        Login("admin", "admin@businessasusual.com", "Admin User", "Administrator");
    }

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

    public void Logout()
    {
        _currentUser = null;
        OnAuthStateChanged?.Invoke();
    }

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

    public void UpdatePreferences(UserPreferences preferences)
    {
        if (_currentUser != null)
        {
            _currentUser.Preferences = preferences;
            OnAuthStateChanged?.Invoke();
        }
    }
}

public class UserSession
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public DateTime LoginTime { get; set; }
    public UserPreferences Preferences { get; set; } = new();
}

public class UserPreferences
{
    public string Language { get; set; } = "en-US";
    public string TimeZone { get; set; } = "UTC";
    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public string TimeFormat { get; set; } = "12h";
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool DarkMode { get; set; } = false;
    public int ItemsPerPage { get; set; } = 25;
}
