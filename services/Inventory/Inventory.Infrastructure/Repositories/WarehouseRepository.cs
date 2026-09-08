using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Application.Services;

namespace Inventory.Infrastructure.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly InventoryDbContext _context;
    private readonly ITenantContext _tenantContext;

    public WarehouseRepository(InventoryDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
        => await _context.Warehouses.Where(w => w.CompanyId == _tenantContext.CompanyId && w.IsActive).ToListAsync();

    public async Task<Warehouse?> GetByIdAsync(Guid id)
        => await _context.Warehouses
            .Include(w => w.BinLocations)
            .FirstOrDefaultAsync(w => w.Id == id);

    public async Task<Warehouse?> GetByCodeAsync(string code)
        => await _context.Warehouses.FirstOrDefaultAsync(w => w.Code == code);

    public async Task<Warehouse> AddAsync(Warehouse entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CompanyId = _tenantContext.CompanyId;
        entity.CreatedAt = DateTime.UtcNow;
        _context.Warehouses.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Warehouse> UpdateAsync(Warehouse entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Warehouses.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Warehouses.FindAsync(id);
        if (entity != null)
        {
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
