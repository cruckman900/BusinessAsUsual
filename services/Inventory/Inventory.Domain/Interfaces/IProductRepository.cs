using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySkuAsync(string sku);
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<IEnumerable<Product>> GetLowStockAsync();
    Task<Product> AddAsync(Product entity);
    Task<Product> UpdateAsync(Product entity);
    Task DeleteAsync(Guid id);
}
