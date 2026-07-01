namespace HR.Contracts.Navigation;

/// <summary>
/// Defines the navigation structure for the HR module in mobile apps
/// </summary>
public class HRNavigationMap
{
    public string ModuleId { get; set; } = "hr";
    public string ModuleName { get; set; } = "Human Resources";
    public string Icon { get; set; } = "person";
    public List<NavigationItem> Items { get; set; } = new();
}

/// <summary>
/// A navigation item within the module
/// </summary>
public class NavigationItem
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Screen { get; set; } = string.Empty; // employee-list, employee-detail, etc.
    public string? Route { get; set; }
    public List<NavigationItem>? Children { get; set; }
    public bool RequiresPermission { get; set; } = false;
    public string? Permission { get; set; }
}
