using ModuleRegistry.Application.DTOs;
using ModuleRegistry.Domain.Entities;
using ModuleRegistry.Domain.Repositories;

namespace ModuleRegistry.Application.Services;

public class ModuleRegistryService : IModuleRegistryService
{
    private readonly IModuleRepository _repository;

    public ModuleRegistryService(IModuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
    {
        var modules = await _repository.GetAllAsync();
        return modules.Select(MapToDto);
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(string moduleId)
    {
        var module = await _repository.GetByModuleIdAsync(moduleId);
        return module != null ? MapToDto(module) : null;
    }

    public async Task<IEnumerable<ModuleDto>> GetActiveModulesAsync()
    {
        var modules = await _repository.GetActiveAsync();
        return modules.Select(MapToDto);
    }

    public async Task<IEnumerable<ModuleDto>> GetModulesWithUiAsync()
    {
        var modules = await _repository.GetModulesWithUiAsync();
        return modules.Select(MapToDto);
    }

    public async Task<IEnumerable<ModuleDto>> GetModulesWithMobileAsync()
    {
        var modules = await _repository.GetModulesWithMobileAsync();
        return modules.Select(MapToDto);
    }

    public async Task RegisterModuleAsync(RegisterModuleRequest request)
    {
        var module = new ModuleMetadata
        {
            Id = Guid.NewGuid(),
            ModuleId = request.ModuleId,
            Key = request.Key,
            DisplayName = request.DisplayName,
            Description = request.Description,
            Version = request.Version,
            ApiBaseUrl = request.ApiBaseUrl,
            UiEntryPoint = request.UiEntryPoint,
            Icon = request.Icon,
            Permissions = request.Permissions,
            Capabilities = request.Capabilities,
            HealthUrl = request.HealthUrl,
            TenantMode = request.TenantMode,
            IsActive = true,
            RegisteredAt = DateTime.UtcNow,
            LastHealthCheck = DateTime.UtcNow,
            HealthStatus = "Unknown",
            MobileUISpecUrl = request.MobileUISpecUrl,
            MobileContractVersion = request.MobileContractVersion,
            SupportsMobile = request.SupportsMobile,
            NavigationItems = request.NavigationItems.Select(nav => MapToNavigationItem(nav)).ToList()
        };

        await _repository.AddOrUpdateAsync(module);
    }

    private static ModuleRegistry.Domain.Entities.NavigationItem MapToNavigationItem(NavigationItemDto dto)
    {
        return new ModuleRegistry.Domain.Entities.NavigationItem
        {
            Label = dto.Label,
            Route = dto.Route,
            Icon = dto.Icon,
            ExpandedByDefault = dto.ExpandedByDefault,
            Children = dto.Children?.Select(MapToNavigationItem).ToList()
        };
    }

    private static ModuleDto MapToDto(ModuleMetadata module)
    {
        return new ModuleDto
        {
            ModuleId = module.ModuleId,
            Key = module.Key,
            DisplayName = module.DisplayName,
            Description = module.Description,
            Version = module.Version,
            ApiBaseUrl = module.ApiBaseUrl,
            UiEntryPoint = module.UiEntryPoint,
            Icon = module.Icon,
            Permissions = module.Permissions,
            Capabilities = module.Capabilities,
            HealthUrl = module.HealthUrl,
            TenantMode = module.TenantMode,
            IsActive = module.IsActive,
            HealthStatus = module.HealthStatus,
            RegisteredAt = module.RegisteredAt,
            LastHealthCheck = module.LastHealthCheck,
            MobileUISpecUrl = module.MobileUISpecUrl,
            MobileContractVersion = module.MobileContractVersion,
            SupportsMobile = module.SupportsMobile,
            NavigationItems = module.NavigationItems.Select(nav => MapToNavigationItemDto(nav)).ToList()
        };
    }

    private static NavigationItemDto MapToNavigationItemDto(ModuleRegistry.Domain.Entities.NavigationItem item)
    {
        return new NavigationItemDto
        {
            Label = item.Label,
            Route = item.Route,
            Icon = item.Icon,
            ExpandedByDefault = item.ExpandedByDefault,
            Children = item.Children?.Select(MapToNavigationItemDto).ToList()
        };
    }
}
