namespace Platform.Web.Services;

public class SmartDefaultsService
{
    // Stores user preferences and smart defaults
    private readonly Dictionary<string, object> _defaults = new();

    // Remember user's last input for prefill
    public void RememberValue(string key, object value)
    {
        _defaults[key] = value;
    }

    // Get remembered value or default
    public T? GetValue<T>(string key, T? fallback = default)
    {
        if (_defaults.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return fallback;
    }

    // Clear a specific remembered value
    public void ClearValue(string key)
    {
        _defaults.Remove(key);
    }

    // Clear all remembered values
    public void ClearAll()
    {
        _defaults.Clear();
    }

    // Check if a value is remembered
    public bool HasValue(string key)
    {
        return _defaults.ContainsKey(key);
    }

    // Smart defaults for common scenarios
    public class UserDefaults
    {
        public string DefaultRole { get; set; } = "User";
        public bool DefaultActive { get; set; } = true;
        public string? DefaultDepartment { get; set; }
        public string? DefaultTimezone { get; set; }
    }

    public class FormDefaults
    {
        public DateTime DefaultDate { get; set; } = DateTime.Today;
        public string DefaultCurrency { get; set; } = "USD";
        public string DefaultLanguage { get; set; } = "en-US";
    }

    // Get smart defaults based on context
    public UserDefaults GetUserDefaults()
    {
        return new UserDefaults
        {
            DefaultRole = GetValue("user.lastRole", "User"),
            DefaultActive = GetValue("user.defaultActive", true),
            DefaultDepartment = GetValue<string?>("user.lastDepartment", null),
            DefaultTimezone = GetValue("user.timezone", TimeZoneInfo.Local.Id)
        };
    }

    public FormDefaults GetFormDefaults()
    {
        return new FormDefaults
        {
            DefaultDate = GetValue("form.lastDate", DateTime.Today),
            DefaultCurrency = GetValue("form.currency", "USD"),
            DefaultLanguage = GetValue("form.language", "en-US")
        };
    }

    // Prefill helpers
    public void RememberUserFormData(string role, string? department, bool isActive)
    {
        RememberValue("user.lastRole", role);
        if (!string.IsNullOrEmpty(department))
            RememberValue("user.lastDepartment", department);
        RememberValue("user.defaultActive", isActive);
    }

    public void RememberFormData(DateTime date, string currency, string language)
    {
        RememberValue("form.lastDate", date);
        RememberValue("form.currency", currency);
        RememberValue("form.language", language);
    }
}
