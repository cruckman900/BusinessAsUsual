using ModuleRegistry.Domain.Entities;

namespace ModuleRegistry.Domain.Repositories;

public interface IModuleRepository
{
    Task<IEnumerable<ModuleMetadata>> GetAllAsync();
    Task<ModuleMetadata?> GetByModuleIdAsync(string moduleId);
    Task<IEnumerable<ModuleMetadata>> GetActiveAsync();
    Task<IEnumerable<ModuleMetadata>> GetModulesWithUiAsync();
    Task<IEnumerable<ModuleMetadata>> GetModulesWithMobileAsync();
    Task AddOrUpdateAsync(ModuleMetadata module);
    Task UpdateHealthStatusAsync(string moduleId, string status, DateTime lastChecked);
}
