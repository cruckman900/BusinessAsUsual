using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IInventoryTransactionRepository
{
    Task<IEnumerable<InventoryTransaction>> GetAllAsync();
    Task<InventoryTransaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<InventoryTransaction> AddAsync(InventoryTransaction entity);
}
