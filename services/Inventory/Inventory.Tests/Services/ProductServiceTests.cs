using FluentAssertions;
using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Moq;

namespace Inventory.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IStockItemRepository> _stockItemRepositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _stockItemRepositoryMock = new Mock<IStockItemRepository>();
        _service = new ProductService(_productRepositoryMock.Object, _stockItemRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", SKU = "SKU001", Price = 10.00m, IsActive = true },
            new() { Id = Guid.NewGuid(), Name = "Product 2", SKU = "SKU002", Price = 20.00m, IsActive = true }
        };
        _productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);
        _stockItemRepositoryMock.Setup(x => x.GetByProductIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<StockItem>());

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Product 1");
        result.Should().Contain(p => p.Name == "Product 2");
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product 
        { 
            Id = productId, 
            Name = "Test Product", 
            SKU = "TEST001", 
            Price = 15.99m,
            IsActive = true 
        };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        _stockItemRepositoryMock.Setup(x => x.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<StockItem> { new() { QuantityOnHand = 100, QuantityAllocated = 0 } });

        // Act
        var result = await _service.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        result.TotalStock.Should().Be(100);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductByIdAsync(productId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_CreatesProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            SKU = "NEW001",
            Price = 25.50m,
            Cost = 15.00m,
            Category = "Electronics"
        };
        var createdProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            SKU = createDto.SKU,
            Price = createDto.Price,
            Cost = createDto.Cost,
            Category = createDto.Category,
            IsActive = true
        };
        _productRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);
        _stockItemRepositoryMock.Setup(x => x.GetByProductIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<StockItem>());

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Product");
        result.SKU.Should().Be("NEW001");
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidId_UpdatesProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = new Product 
        { 
            Id = productId, 
            Name = "Old Name", 
            SKU = "OLD001",
            Price = 10.00m,
            IsActive = true 
        };
        var updateDto = new UpdateProductDto
        {
            Id = productId,
            Name = "Updated Name",
            SKU = "OLD001",
            Price = 12.00m
        };

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(existingProduct);
        _stockItemRepositoryMock.Setup(x => x.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<StockItem>());

        // Act
        var result = await _service.UpdateProductAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_DeletesProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(x => x.DeleteAsync(productId)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteProductAsync(productId);

        // Assert
        _productRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAllProductsAsync_CalculatesTotalStock_Correctly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var products = new List<Product>
        {
            new() { Id = productId, Name = "Stocked Product", SKU = "STOCK001", Price = 100m, IsActive = true }
        };
        var stockItems = new List<StockItem>
        {
            new() { ProductId = productId, QuantityOnHand = 50, QuantityAllocated = 0 },
            new() { ProductId = productId, QuantityOnHand = 30, QuantityAllocated = 0 },
            new() { ProductId = productId, QuantityOnHand = 20, QuantityAllocated = 0 }
        };

        _productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);
        _stockItemRepositoryMock.Setup(x => x.GetByProductIdAsync(productId)).ReturnsAsync(stockItems);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        var product = result.First();
        product.TotalStock.Should().Be(100); // 50 + 30 + 20
    }

    [Fact]
    public async Task SearchProductsAsync_ReturnsMatchingProducts()
    {
        // Arrange
        var searchTerm = "laptop";
        var matchingProducts = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Gaming Laptop", SKU = "LAP001", Price = 1200m, IsActive = true },
            new() { Id = Guid.NewGuid(), Name = "Business Laptop", SKU = "LAP002", Price = 800m, IsActive = true }
        };

        _productRepositoryMock.Setup(x => x.SearchAsync(searchTerm)).ReturnsAsync(matchingProducts);
        _stockItemRepositoryMock.Setup(x => x.GetByProductIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<StockItem>());

        // Act
        var result = await _service.SearchProductsAsync(searchTerm);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Name.Contains("Laptop", StringComparison.OrdinalIgnoreCase));
    }
}
