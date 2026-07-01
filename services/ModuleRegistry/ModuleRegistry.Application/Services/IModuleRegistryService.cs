using ModuleRegistry.Application.DTOs;

namespace ModuleRegistry.Application.Services;

public interface IModuleRegistryService
{
    Task<IEnumerable<ModuleDto>> GetAllModulesAsync();
    Task<ModuleDto?> GetModuleByIdAsync(string moduleId);
    Task<IEnumerable<ModuleDto>> GetActiveModulesAsync();
    Task<IEnumerable<ModuleDto>> GetModulesWithUiAsync();
    Task<IEnumerable<ModuleDto>> GetModulesWithMobileAsync();
    Task RegisterModuleAsync(RegisterModuleRequest request);
}
