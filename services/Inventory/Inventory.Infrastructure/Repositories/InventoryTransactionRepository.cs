using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly InventoryDbContext _context;

    public InventoryTransactionRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryTransaction>> GetAllAsync()
        => await _context.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.BinLocation)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

    public async Task<InventoryTransaction?> GetByIdAsync(Guid id)
        => await _context.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.BinLocation)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId)
        => await _context.InventoryTransactions
            .Include(t => t.Warehouse)
            .Include(t => t.BinLocation)
            .Where(t => t.ProductId == productId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

    public async Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(Guid warehouseId)
        => await _context.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.BinLocation)
            .Where(t => t.WarehouseId == warehouseId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

    public async Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        => await _context.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.BinLocation)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

    public async Task<InventoryTransaction> AddAsync(InventoryTransaction entity)
    {
        entity.Id = Guid.NewGuid();
        _context.InventoryTransactions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
