namespace ModuleRegistry.Application.DTOs;

/// <summary>
/// Represents a module registered in the system, including its metadata, capabilities, and navigation items.
/// </summary>
public class ModuleDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the module.
    /// </summary>
    public string ModuleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group name for the module, used for categorization.
    /// </summary>
    public string Key { get; set; } = string.Empty; // Short key for routing (e.g., "hr", "finance")

    /// <summary>
    /// Gets or sets the display name of the module, which is used for user-friendly representation in the UI.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the module, providing additional context about its purpose and functionality.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the module, indicating its release or build number.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL for the module's API, which is used for making HTTP requests to the module's backend services.
    /// </summary>
    public string ApiBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entry point URL for the module's UI, which is used for rendering the module's frontend interface.
    /// </summary>
    public string? UiEntryPoint { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the module, which is used for visual representation in the UI.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the list of permissions required to access the module, which is used for authorization and access control.
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of capabilities provided by the module, which is used for feature discovery and integration with other modules.
    /// </summary>
    public List<string> Capabilities { get; set; } = new();

    /// <summary>
    /// Gets or sets the URL for checking the health status of the module.
    /// </summary>
    public string HealthUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant mode for the module, which indicates how the module handles multi-tenancy (e.g., "tenant-per-database", "shared-database").
    /// </summary>
    public string TenantMode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the module is currently active and operational.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the health status of the module, which indicates whether the module is healthy, degraded, or unhealthy.
    /// </summary>
    public string HealthStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the module was registered in the system.
    /// </summary>
    public DateTime RegisteredAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last health check performed on the module, which is used for monitoring and diagnostics.
    /// </summary>
    public DateTime LastHealthCheck { get; set; }

    // Mobile UI Contract Support

    /// <summary>
    /// Gets or sets the URL for the mobile UI specification of the module, which is used for rendering the module's interface on mobile devices.
    /// </summary>
    public string? MobileUISpecUrl { get; set; }

    /// <summary>
    /// Gets or sets the version of the mobile contract for the module, which is used for ensuring compatibility with mobile clients.
    /// </summary>
    public string? MobileContractVersion { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the module supports mobile clients, which is used for determining if the module can be accessed from mobile devices.
    /// </summary>
    public bool SupportsMobile { get; set; }

    // Navigation Items for Sidebar

    /// <summary>
    /// Gets or sets the list of navigation items for the module, which is used for rendering the module's navigation menu in the UI.
    /// </summary>
    public List<NavigationItemDto> NavigationItems { get; set; } = new();
}

/// <summary>
/// Represents a navigation item for a module, including its label, route, and optional icon.
/// </summary>
public class NavigationItemDto
{
    /// <summary>
    /// Gets or sets the label for the navigation item, which is used for display in the UI.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the route for the navigation item, which is used for navigating to the corresponding screen or page in the module.
    /// </summary>
    public string Route { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon for the navigation item, which is used for visual representation in the UI.
    /// </summary>
    public string? Icon { get; set; }
}
