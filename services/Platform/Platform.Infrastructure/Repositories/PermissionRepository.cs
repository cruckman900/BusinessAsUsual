using Microsoft.EntityFrameworkCore;
using Platform.Domain.Entities;
using Platform.Domain.Interfaces;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly PlatformDbContext _context;

    public PermissionRepository(PlatformDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<Permission> AddAsync(Permission permission)
    {
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
            .Where(p => p.RolePermissions.Any(rp => rp.RoleId == roleId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(string module)
    {
        return await _context.Permissions
            .Where(p => p.Module == module)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Permissions.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Permissions.AnyAsync(p => p.Name == name);
    }
}
