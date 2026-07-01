namespace ModuleRegistry.Domain.Entities;

/// <summary>
/// Represents a registered module in the Business As Usual ecosystem.
/// </summary>
public class ModuleMetadata
{
    public Guid Id { get; set; }
    public string ModuleId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty; // Short key for routing (e.g., "hr", "finance")
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string? UiEntryPoint { get; set; }
    public string? Icon { get; set; }
    public List<string> Permissions { get; set; } = new();
    public List<string> Capabilities { get; set; } = new();
    public string HealthUrl { get; set; } = string.Empty;
    public string TenantMode { get; set; } = "tenant-per-database";
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredAt { get; set; }
    public DateTime LastHealthCheck { get; set; }
    public string HealthStatus { get; set; } = "Unknown";

    // Mobile UI Contract Support
    public string? MobileUISpecUrl { get; set; }
    public string? MobileContractVersion { get; set; }
    public bool SupportsMobile { get; set; } = false;

    // Navigation Items for Sidebar
    public List<NavigationItem> NavigationItems { get; set; } = new();
}

public class NavigationItem
{
    public string Label { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? Icon { get; set; }
}
