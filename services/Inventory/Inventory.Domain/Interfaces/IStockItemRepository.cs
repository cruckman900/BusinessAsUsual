using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IStockItemRepository
{
    Task<IEnumerable<StockItem>> GetAllAsync();
    Task<StockItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<StockItem>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<StockItem>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<StockItem?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId);
    Task<StockItem> AddAsync(StockItem entity);
    Task<StockItem> UpdateAsync(StockItem entity);
    Task DeleteAsync(Guid id);
}
