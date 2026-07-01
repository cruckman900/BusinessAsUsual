namespace BusinessAsUsual.Web.Services;

/// <summary>
/// Represents a data transfer object (DTO) for module information, including its unique identifier, display name, description, version, API base URL, UI entry point, icon, permissions, capabilities, health status, and navigation items. This DTO is used to transfer module metadata between the module discovery service and other components of the application.
/// </summary>
public class ModuleDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the module, which is used to distinguish it from other modules in the system. This property is intended to be a globally unique identifier (GUID) or a similar unique string that can be used to reference the module in various contexts, such as routing, configuration, and module management.
    /// </summary>
    public string ModuleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group name for the module, which is used for categorization and routing purposes. This property is intended to provide a short and unique key that can be used to identify the module within the application, allowing for easier navigation and organization of modules.
    /// </summary>
    public string Key { get; set; } = string.Empty; // Short key for routing (e.g., "hr", "finance")

    /// <summary>
    /// Gets or sets the display name of the module, which is used for user-friendly representation in the UI. This property is intended to provide a clear and concise name for the module that can be easily recognized by users and developers alike.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the module, providing additional context about its purpose and functionality. This property is used to give users and developers a better understanding of what the module does and how it fits into the overall application architecture.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the module, indicating its release or build number. This property is used to track the version of the module and can be useful for compatibility checks, updates, and ensuring that the correct version of the module is being used in the application.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL for the module's API, which is used for making HTTP requests to the module's backend services. This property allows other components of the application to interact with the module's API endpoints, enabling communication and data exchange between different modules and services.
    /// </summary>
    public string ApiBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entry point URL for the module's UI, which is used for rendering the module's frontend interface. This property can be used to navigate to the module's user interface from other parts of the application.
    /// </summary>
    public string? UiEntryPoint { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the module, which is used for visual representation in the UI. This property can be used to display an icon next to the module's name in the user interface, providing a more intuitive and visually appealing experience for users.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the list of permissions required to access the module, which is used for authorization and access control. This property allows the application to determine what actions or resources a user can access based on their assigned permissions, ensuring that users only have access to the functionality they are authorized to use.
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of capabilities provided by the module, which is used for feature discovery and integration with other modules. This property allows other components of the application to determine what features or functionalities the module supports, enabling better interoperability and collaboration between different modules.
    /// </summary>
    public List<string> Capabilities { get; set; } = new();

    /// <summary>
    /// Gets or sets the URL for checking the health status of the module. This property is used to determine if the module is currently healthy and operational, and it can be used for monitoring purposes or to display the module's health status in the UI.
    /// </summary>
    public string HealthUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant mode for the module, which indicates how the module handles multi-tenancy. Possible values include "tenant-per-database", "tenant-per-schema", and "single-tenant". This property is used to determine how the module should be configured and deployed in a multi-tenant environment.
    /// </summary>
    public string TenantMode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the module is currently active and available for use. This property can be used to determine if the module should be displayed in the UI or if it should be hidden from users.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the health status of the module, which indicates whether the module is currently healthy and operational. This property can be used to display the module's health status in the UI or for monitoring purposes.
    /// </summary>
    public string HealthStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of navigation items for the module, which define the structure and routes for navigating within the module's UI. Each navigation item includes a label, route, and optional icon.
    /// </summary>
    public List<NavigationItemDto> NavigationItems { get; set; } = new();
}

/// <summary>
/// Represents a data transfer object (DTO) for a navigation item, including its label, route, and optional icon. This DTO is used to define the navigation structure for modules in the application, allowing users to navigate between different sections of the module's UI.
/// </summary>
public class NavigationItemDto
{
    /// <summary>
    /// Gets or sets the label for the navigation item, which is used for display in the UI. This property is required and should not be null or empty.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the route associated with the navigation item, which is used for navigation within the application. This property is required and should not be null or empty.
    /// </summary>
    public string Route { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon associated with the navigation item, which is used for display in the UI. This property is optional and can be null if no icon is specified.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the optional child navigation items for hierarchical menus.
    /// </summary>
    public List<NavigationItemDto>? Children { get; set; }

    /// <summary>
    /// Gets or sets whether this navigation group should be expanded by default.
    /// </summary>
    public bool ExpandedByDefault { get; set; } = true;
}
