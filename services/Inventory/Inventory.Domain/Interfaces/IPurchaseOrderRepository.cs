using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<IEnumerable<PurchaseOrder>> GetAllAsync();
    Task<PurchaseOrder?> GetByIdAsync(Guid id);
    Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId);
    Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status);
    Task<PurchaseOrder> AddAsync(PurchaseOrder entity);
    Task<PurchaseOrder> UpdateAsync(PurchaseOrder entity);
    Task DeleteAsync(Guid id);
}
