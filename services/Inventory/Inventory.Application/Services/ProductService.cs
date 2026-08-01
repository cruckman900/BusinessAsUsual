using Inventory.Application.DTOs;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockItemRepository _stockItemRepository;

    public ProductService(IProductRepository productRepository, IStockItemRepository stockItemRepository)
    {
        _productRepository = productRepository;
        _stockItemRepository = stockItemRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var productDtos = new List<ProductDto>();

        foreach (var product in products)
        {
            var stockItems = await _stockItemRepository.GetByProductIdAsync(product.Id);
            var totalStock = stockItems.Sum(s => s.QuantityAvailable);

            productDtos.Add(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Barcode = product.Barcode,
                Cost = product.Cost,
                Price = product.Price,
                ReorderPoint = product.ReorderPoint,
                ReorderQuantity = product.ReorderQuantity,
                Category = product.Category,
                UnitOfMeasure = product.UnitOfMeasure,
                IsActive = product.IsActive,
                IsTrackedInventory = product.IsTrackedInventory,
                ImageUrl = product.ImageUrl,
                TotalStock = totalStock
            });
        }

        return productDtos;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        var stockItems = await _stockItemRepository.GetByProductIdAsync(product.Id);
        var totalStock = stockItems.Sum(s => s.QuantityAvailable);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Cost = product.Cost,
            Price = product.Price,
            ReorderPoint = product.ReorderPoint,
            ReorderQuantity = product.ReorderQuantity,
            Category = product.Category,
            UnitOfMeasure = product.UnitOfMeasure,
            IsActive = product.IsActive,
            IsTrackedInventory = product.IsTrackedInventory,
            ImageUrl = product.ImageUrl,
            TotalStock = totalStock
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
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
            IsTrackedInventory = dto.IsTrackedInventory,
            ImageUrl = dto.ImageUrl
        };

        var created = await _productRepository.AddAsync(product);

        return new ProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            SKU = created.SKU,
            Barcode = created.Barcode,
            Cost = created.Cost,
            Price = created.Price,
            ReorderPoint = created.ReorderPoint,
            ReorderQuantity = created.ReorderQuantity,
            Category = created.Category,
            UnitOfMeasure = created.UnitOfMeasure,
            IsActive = created.IsActive,
            IsTrackedInventory = created.IsTrackedInventory,
            ImageUrl = created.ImageUrl,
            TotalStock = 0
        };
    }

    public async Task<ProductDto> UpdateProductAsync(UpdateProductDto dto)
    {
        var existing = await _productRepository.GetByIdAsync(dto.Id);
        if (existing == null)
            throw new Exception($"Product with ID {dto.Id} not found");

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.SKU = dto.SKU;
        existing.Barcode = dto.Barcode;
        existing.Cost = dto.Cost;
        existing.Price = dto.Price;
        existing.ReorderPoint = dto.ReorderPoint;
        existing.ReorderQuantity = dto.ReorderQuantity;
        existing.Category = dto.Category;
        existing.UnitOfMeasure = dto.UnitOfMeasure;
        existing.IsActive = dto.IsActive;
        existing.IsTrackedInventory = dto.IsTrackedInventory;
        existing.ImageUrl = dto.ImageUrl;

        var updated = await _productRepository.UpdateAsync(existing);
        var stockItems = await _stockItemRepository.GetByProductIdAsync(updated.Id);
        var totalStock = stockItems.Sum(s => s.QuantityAvailable);

        return new ProductDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            SKU = updated.SKU,
            Barcode = updated.Barcode,
            Cost = updated.Cost,
            Price = updated.Price,
            ReorderPoint = updated.ReorderPoint,
            ReorderQuantity = updated.ReorderQuantity,
            Category = updated.Category,
            UnitOfMeasure = updated.UnitOfMeasure,
            IsActive = updated.IsActive,
            IsTrackedInventory = updated.IsTrackedInventory,
            ImageUrl = updated.ImageUrl,
            TotalStock = totalStock
        };
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchAsync(searchTerm);
        var productDtos = new List<ProductDto>();

        foreach (var product in products)
        {
            var stockItems = await _stockItemRepository.GetByProductIdAsync(product.Id);
            var totalStock = stockItems.Sum(s => s.QuantityAvailable);

            productDtos.Add(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Barcode = product.Barcode,
                Cost = product.Cost,
                Price = product.Price,
                ReorderPoint = product.ReorderPoint,
                ReorderQuantity = product.ReorderQuantity,
                Category = product.Category,
                UnitOfMeasure = product.UnitOfMeasure,
                IsActive = product.IsActive,
                IsTrackedInventory = product.IsTrackedInventory,
                ImageUrl = product.ImageUrl,
                TotalStock = totalStock
            });
        }

        return productDtos;
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
    {
        var products = await _productRepository.GetLowStockAsync();
        var productDtos = new List<ProductDto>();

        foreach (var product in products)
        {
            var stockItems = await _stockItemRepository.GetByProductIdAsync(product.Id);
            var totalStock = stockItems.Sum(s => s.QuantityAvailable);

            productDtos.Add(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Barcode = product.Barcode,
                Cost = product.Cost,
                Price = product.Price,
                ReorderPoint = product.ReorderPoint,
                ReorderQuantity = product.ReorderQuantity,
                Category = product.Category,
                UnitOfMeasure = product.UnitOfMeasure,
                IsActive = product.IsActive,
                IsTrackedInventory = product.IsTrackedInventory,
                ImageUrl = product.ImageUrl,
                TotalStock = totalStock
            });
        }

        return productDtos;
    }
}
