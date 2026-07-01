namespace BusinessAsUsual.Web.Modules._Shared
{
    /// <summary>
    /// Represents the definition of a module, including its name, icon, route, and optional description.
    /// </summary>
    public class ModuleDefinition
    {
        /// <summary>
        /// Gets or sets the unique key/identifier for the module.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the icon associated with the item.
        /// </summary>
        public string Icon { get; set; } = "";

        /// <summary>
        /// Gets or sets the route template associated with the current resource.
        /// </summary>
        public string Route { get; set; } = "";

        /// <summary>
        /// Gets or sets the description associated with the object.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the navigation items for the module's sidebar.
        /// </summary>
        public List<ModuleNavigationItem> NavigationItems { get; set; } = new();
    }

    /// <summary>
    /// Represents a navigation item for a module.
    /// </summary>
    public class ModuleNavigationItem
    {
        /// <summary>
        /// Gets or sets the display label for the navigation item.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the route path for the navigation item.
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional icon for the navigation item.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Gets or sets the optional child navigation items for hierarchical menus.
        /// </summary>
        public List<ModuleNavigationItem>? Children { get; set; }

        /// <summary>
        /// Gets or sets whether this navigation group should be expanded by default.
        /// </summary>
        public bool ExpandedByDefault { get; set; } = true;
    }

}
