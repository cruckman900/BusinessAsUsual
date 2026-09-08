using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Application.Services;

namespace Inventory.Infrastructure.Repositories;

public class StockItemRepository : IStockItemRepository
{
    private readonly InventoryDbContext _context;
    private readonly ITenantContext _tenantContext;

    public StockItemRepository(InventoryDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<StockItem>> GetAllAsync()
        => await _context.StockItems
            .Where(s => s.CompanyId == _tenantContext.CompanyId)
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Include(s => s.BinLocation)
            .ToListAsync();

    public async Task<StockItem?> GetByIdAsync(Guid id)
        => await _context.StockItems
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Include(s => s.BinLocation)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<StockItem>> GetByProductIdAsync(Guid productId)
        => await _context.StockItems
            .Include(s => s.Warehouse)
            .Include(s => s.BinLocation)
            .Where(s => s.ProductId == productId)
            .ToListAsync();

    public async Task<IEnumerable<StockItem>> GetByWarehouseIdAsync(Guid warehouseId)
        => await _context.StockItems
            .Include(s => s.Product)
            .Include(s => s.BinLocation)
            .Where(s => s.WarehouseId == warehouseId)
            .ToListAsync();

    public async Task<StockItem?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId)
        => await _context.StockItems
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Include(s => s.BinLocation)
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);

    public async Task<StockItem> AddAsync(StockItem entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CompanyId = _tenantContext.CompanyId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.LastStockDate = DateTime.UtcNow;
        _context.StockItems.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<StockItem> UpdateAsync(StockItem entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.LastStockDate = DateTime.UtcNow;
        _context.StockItems.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.StockItems.FindAsync(id);
        if (entity != null)
        {
            _context.StockItems.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
