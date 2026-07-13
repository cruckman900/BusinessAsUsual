namespace CRM.Contracts.Navigation;

/// <summary>
/// Defines the navigation structure for a BAU module in mobile apps.
/// Mirrors the shape the Android client expects from every module's ui-spec.
/// </summary>
public class ModuleNavigationMap
{
    public string ModuleId { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Icon { get; set; } = "widgets";
    public List<NavigationItem> Items { get; set; } = new();
}

/// <summary>
/// A navigation item within the module. Each item maps a sidebar entry to a
/// registered screen key (e.g. "lead-list") and an optional web route.
/// </summary>
public class NavigationItem
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Screen { get; set; } = string.Empty;
    public string? Route { get; set; }
    public List<NavigationItem>? Children { get; set; }
    public bool RequiresPermission { get; set; } = false;
    public string? Permission { get; set; }
}
