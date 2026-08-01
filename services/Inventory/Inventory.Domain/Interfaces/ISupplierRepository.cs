using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(Guid id);
    Task<Supplier?> GetByCodeAsync(string code);
    Task<IEnumerable<Supplier>> SearchAsync(string searchTerm);
    Task<Supplier> AddAsync(Supplier entity);
    Task<Supplier> UpdateAsync(Supplier entity);
    Task DeleteAsync(Guid id);
}
