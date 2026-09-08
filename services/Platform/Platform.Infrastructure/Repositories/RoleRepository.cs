using Microsoft.EntityFrameworkCore;
using Platform.Domain.Entities;
using Platform.Domain.Interfaces;
using Platform.Infrastructure.Data;
using BusinessAsUsual.Application.Services;

namespace Platform.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly PlatformDbContext _context;
    private readonly ITenantContext _tenantContext;

    public RoleRepository(PlatformDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.CompanyId == _tenantContext.CompanyId || r.IsSystemRole)
            .ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id && (r.CompanyId == _tenantContext.CompanyId || r.IsSystemRole));
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Name == name && (r.CompanyId == _tenantContext.CompanyId || r.IsSystemRole));
    }

    public async Task<Role> AddAsync(Role role)
    {
        role.CompanyId = _tenantContext.CompanyId;
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        role.UpdatedAt = DateTime.UtcNow;
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<IEnumerable<Role>> GetRolesByUserAsync(Guid userId)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.UserRoles.Any(ur => ur.UserId == userId) && (r.CompanyId == _tenantContext.CompanyId || r.IsSystemRole))
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == _tenantContext.CompanyId);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Roles.AnyAsync(r => r.Id == id && (r.CompanyId == _tenantContext.CompanyId || r.IsSystemRole));
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Roles.AnyAsync(r => r.Name == name && (r.CompanyId == _tenantContext.CompanyId || r.IsSystemRole));
    }
}
