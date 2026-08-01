namespace Inventory.Contracts.Mobile;

public class ModuleNavigationMap
{
    public string ModuleName { get; set; } = "Inventory";
    public string ModuleIcon { get; set; } = "inventory_2";
    public int DisplayOrder { get; set; } = 60;
    public string BaseRoute { get; set; } = "/inventory";
    public List<NavigationItem> NavigationItems { get; set; } = new();
}

public class NavigationItem
{
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public List<NavigationItem>? Children { get; set; }
}
