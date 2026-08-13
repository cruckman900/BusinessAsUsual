using Inventory.Application.DTOs;

namespace Inventory.Application.Services;

/// <summary>
/// Unified service interface for Inventory module operations in shell environments.
/// Provides essential product, stock, warehouse, supplier, and purchase order management operations.
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
    Task<IEnumerable<StockItemDto>> GetAllStockItemsAsync();

    // Warehouse operations
    Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
    Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id);
    Task<bool> DeleteWarehouseAsync(Guid id);

    // Supplier operations
    Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
    Task<bool> DeleteSupplierAsync(Guid id);

    // Purchase Order operations
    Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();
    Task<bool> DeletePurchaseOrderAsync(Guid id);
    Task<PurchaseOrderDto> ReceivePurchaseOrderAsync(Guid id);

    // Inventory Transaction operations
    Task<IEnumerable<InventoryTransactionDto>> GetAllInventoryTransactionsAsync();
}
