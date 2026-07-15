using CRM.Contracts.Navigation;

namespace CRM.Contracts.Specifications;

/// <summary>
/// Root mobile UI specification object returned by a module's ui-spec endpoint.
/// </summary>
public class MobileUISpecification
{
    public string ModuleId { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public ModuleNavigationMap Navigation { get; set; } = new();
    public Dictionary<string, object> Screens { get; set; } = new();
}
