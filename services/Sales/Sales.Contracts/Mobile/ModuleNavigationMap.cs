namespace Sales.Contracts.Mobile;

public class ModuleNavigationMap
{
    public string ModuleName { get; set; } = "Sales";
    public string ModuleIcon { get; set; } = "point_of_sale";
    public int DisplayOrder { get; set; } = 40;
    public string BaseRoute { get; set; } = "/sales";
    public List<NavigationItem> NavigationItems { get; set; } = new();
}

public class NavigationItem
{
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public List<NavigationItem>? Children { get; set; }
}
