using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IWarehouseRepository
{
    Task<IEnumerable<Warehouse>> GetAllAsync();
    Task<Warehouse?> GetByIdAsync(Guid id);
    Task<Warehouse?> GetByCodeAsync(string code);
    Task<Warehouse> AddAsync(Warehouse entity);
    Task<Warehouse> UpdateAsync(Warehouse entity);
    Task DeleteAsync(Guid id);
}
