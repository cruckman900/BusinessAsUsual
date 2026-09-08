using Microsoft.EntityFrameworkCore;
using Platform.Domain.Entities;
using Platform.Domain.Interfaces;
using Platform.Infrastructure.Data;
using BusinessAsUsual.Application.Services;

namespace Platform.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly PlatformDbContext _context;
    private readonly ITenantContext _tenantContext;

    public PermissionRepository(PlatformDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions
            .Where(p => p.CompanyId == _tenantContext.CompanyId || p.IsSystemPermission)
            .ToListAsync();
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id && (p.CompanyId == _tenantContext.CompanyId || p.IsSystemPermission));
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name && (p.CompanyId == _tenantContext.CompanyId || p.IsSystemPermission));
    }

    public async Task<Permission> AddAsync(Permission permission)
    {
        permission.CompanyId = _tenantContext.CompanyId;
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task<Permission> UpdateAsync(Permission permission)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task DeleteAsync(Guid id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission != null)
        {
            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(Guid roleId)
    {
        return await _context.Permissions
            .Where(p => p.RolePermissions.Any(rp => rp.RoleId == roleId) && (p.CompanyId == _tenantContext.CompanyId || p.IsSystemPermission))
            .ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(string module)
    {
        return await _context.Permissions
            .Where(p => p.Module == module && (p.CompanyId == _tenantContext.CompanyId || p.IsSystemPermission))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Permissions.AnyAsync(p => p.Id == id && (p.CompanyId == _tenantContext.CompanyId || p.IsSystemPermission));
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Permissions.AnyAsync(p => p.Name == name && (p.CompanyId == _tenantContext.CompanyId || p.IsSystemPermission));
    }
}
