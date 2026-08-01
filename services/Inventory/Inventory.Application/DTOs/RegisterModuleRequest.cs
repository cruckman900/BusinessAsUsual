namespace Inventory.Application.DTOs;

public class RegisterModuleRequest
{
    public string ModuleId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string? UiEntryPoint { get; set; }
    public string Icon { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public List<string> Capabilities { get; set; } = new();
    public string? HealthUrl { get; set; }
    public string TenantMode { get; set; } = "shared";
    public bool SupportsMobile { get; set; }
    public string? MobileUISpecUrl { get; set; }
    public string? MobileContractVersion { get; set; }
    public List<NavigationItemDto> NavigationItems { get; set; } = new();

    public class NavigationItemDto
    {
        public string Label { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<NavigationItemDto>? Children { get; set; }
    }
}
