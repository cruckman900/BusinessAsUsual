namespace BusinessAsUsual.Web.Modules._Shared
{
    /// <summary>
    /// Represents the definition of a module, including its name, icon, route, and optional description.
    /// </summary>
    public class ModuleDefinition
    {
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
    }
}
