using Microsoft.EntityFrameworkCore;
using ModuleRegistry.Domain.Entities;
using ModuleRegistry.Domain.Repositories;
using ModuleRegistry.Infrastructure.Persistence;

namespace ModuleRegistry.Infrastructure.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly ModuleRegistryDbContext _context;

    public ModuleRepository(ModuleRegistryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ModuleMetadata>> GetAllAsync()
    {
        return await _context.Modules.ToListAsync();
    }

    public async Task<ModuleMetadata?> GetByModuleIdAsync(string moduleId)
    {
        return await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == moduleId);
    }

    public async Task<IEnumerable<ModuleMetadata>> GetActiveAsync()
    {
        return await _context.Modules.Where(m => m.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<ModuleMetadata>> GetModulesWithUiAsync()
    {
        return await _context.Modules
            .Where(m => m.IsActive && m.UiEntryPoint != null)
            .ToListAsync();
    }

    public async Task<IEnumerable<ModuleMetadata>> GetModulesWithMobileAsync()
    {
        return await _context.Modules
            .Where(m => m.IsActive && m.SupportsMobile && m.MobileUISpecUrl != null)
            .ToListAsync();
    }

    public async Task AddOrUpdateAsync(ModuleMetadata module)
    {
        var existing = await GetByModuleIdAsync(module.ModuleId);

        if (existing != null)
        {
            existing.DisplayName = module.DisplayName;
            existing.Description = module.Description;
            existing.Version = module.Version;
            existing.ApiBaseUrl = module.ApiBaseUrl;
            existing.UiEntryPoint = module.UiEntryPoint;
            existing.Icon = module.Icon;
            existing.Permissions = module.Permissions;
            existing.Capabilities = module.Capabilities;
            existing.HealthUrl = module.HealthUrl;
            existing.TenantMode = module.TenantMode;
            existing.IsActive = module.IsActive;
            existing.RegisteredAt = module.RegisteredAt;
            existing.MobileUISpecUrl = module.MobileUISpecUrl;
            existing.MobileContractVersion = module.MobileContractVersion;
            existing.SupportsMobile = module.SupportsMobile;

            _context.Modules.Update(existing);
        }
        else
        {
            await _context.Modules.AddAsync(module);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateHealthStatusAsync(string moduleId, string status, DateTime lastChecked)
    {
        var module = await GetByModuleIdAsync(moduleId);

        if (module != null)
        {
            module.HealthStatus = status;
            module.LastHealthCheck = lastChecked;
            _context.Modules.Update(module);
            await _context.SaveChangesAsync();
        }
    }
}
