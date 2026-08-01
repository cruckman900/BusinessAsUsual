using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly InventoryDbContext _context;

    public PurchaseOrderRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        => await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Warehouse)
            .Include(po => po.Lines)
                .ThenInclude(l => l.Product)
            .ToListAsync();

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
        => await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Warehouse)
            .Include(po => po.Lines)
                .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(po => po.Id == id);

    public async Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber)
        => await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Warehouse)
            .Include(po => po.Lines)
                .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(po => po.OrderNumber == orderNumber);

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId)
        => await _context.PurchaseOrders
            .Include(po => po.Warehouse)
            .Include(po => po.Lines)
            .Where(po => po.SupplierId == supplierId)
            .ToListAsync();

    public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status)
        => await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Warehouse)
            .Include(po => po.Lines)
            .Where(po => po.Status == status)
            .ToListAsync();

    public async Task<PurchaseOrder> AddAsync(PurchaseOrder entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        foreach (var line in entity.Lines)
        {
            line.Id = Guid.NewGuid();
        }
        _context.PurchaseOrders.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PurchaseOrder> UpdateAsync(PurchaseOrder entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.PurchaseOrders.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.PurchaseOrders.FindAsync(id);
        if (entity != null)
        {
            _context.PurchaseOrders.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
