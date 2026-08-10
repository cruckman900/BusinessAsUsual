using Inventory.Application.DTOs;

namespace Inventory.Application.Services;

/// <summary>
/// Unified service interface for Inventory module operations in shell environments.
/// Provides essential product and stock management operations.
/// </summary>
public interface IInventoryService
{
    // Product operations
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductAsync(UpdateProductDto dto);
    Task<bool> DeleteProductAsync(Guid id);

    // Stock operations
    Task<IEnumerable<StockItemDto>> GetStockByProductIdAsync(Guid productId);
    Task<StockSummaryDto?> GetStockSummaryByProductIdAsync(Guid productId);
    Task<IEnumerable<StockItemDto>> GetLowStockItemsAsync();
}
