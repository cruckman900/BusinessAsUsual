using Inventory.Application.DTOs;

namespace Inventory.Application.Services;

/// <summary>
/// Mock implementation of IInventoryService for shell environments where the Inventory API is unavailable.
/// Returns empty/null data to prevent hard failures in the UI.
/// </summary>
public class MockInventoryService : IInventoryService
{
    public Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        return Task.FromResult(Enumerable.Empty<ProductDto>());
    }

    public Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        return Task.FromResult<ProductDto?>(null);
    }

    public Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // Return a minimal product DTO to prevent null reference exceptions
        return Task.FromResult(new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            SKU = dto.SKU,
            Barcode = dto.Barcode,
            Cost = dto.Cost,
            Price = dto.Price,
            ReorderPoint = dto.ReorderPoint,
            ReorderQuantity = dto.ReorderQuantity,
            Category = dto.Category,
            UnitOfMeasure = dto.UnitOfMeasure ?? "EA",
            IsActive = true,
            IsTrackedInventory = dto.IsTrackedInventory,
            ImageUrl = dto.ImageUrl,
            TotalStock = 0
        });
    }

    public Task<ProductDto> UpdateProductAsync(UpdateProductDto dto)
    {
        // Return a minimal product DTO to prevent null reference exceptions
        return Task.FromResult(new ProductDto
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            SKU = dto.SKU,
            Barcode = dto.Barcode,
            Cost = dto.Cost,
            Price = dto.Price,
            ReorderPoint = dto.ReorderPoint,
            ReorderQuantity = dto.ReorderQuantity,
            Category = dto.Category,
            UnitOfMeasure = dto.UnitOfMeasure,
            IsActive = dto.IsActive,
            IsTrackedInventory = dto.IsTrackedInventory,
            ImageUrl = dto.ImageUrl,
            TotalStock = 0
        });
    }

    public Task<bool> DeleteProductAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<StockItemDto>> GetStockByProductIdAsync(Guid productId)
    {
        return Task.FromResult(Enumerable.Empty<StockItemDto>());
    }

    public Task<StockSummaryDto?> GetStockSummaryByProductIdAsync(Guid productId)
    {
        return Task.FromResult<StockSummaryDto?>(null);
    }

    public Task<IEnumerable<StockItemDto>> GetLowStockItemsAsync()
    {
        return Task.FromResult(Enumerable.Empty<StockItemDto>());
    }

    public Task<IEnumerable<StockItemDto>> GetAllStockItemsAsync()
    {
        return Task.FromResult(Enumerable.Empty<StockItemDto>());
    }

    public Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
    {
        return Task.FromResult(Enumerable.Empty<WarehouseDto>());
    }

    public Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id)
    {
        return Task.FromResult<WarehouseDto?>(null);
    }

    public Task<bool> DeleteWarehouseAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        return Task.FromResult(Enumerable.Empty<SupplierDto>());
    }

    public Task<bool> DeleteSupplierAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync()
    {
        return Task.FromResult(Enumerable.Empty<PurchaseOrderDto>());
    }

    public Task<bool> DeletePurchaseOrderAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<PurchaseOrderDto> ReceivePurchaseOrderAsync(Guid id)
    {
        // Return a minimal PO to prevent null refs
        return Task.FromResult(new PurchaseOrderDto
        {
            Id = id,
            OrderNumber = "MOCK-PO",
            Status = "Received"
        });
    }

    public Task<IEnumerable<InventoryTransactionDto>> GetAllInventoryTransactionsAsync()
    {
        return Task.FromResult(Enumerable.Empty<InventoryTransactionDto>());
    }
}
