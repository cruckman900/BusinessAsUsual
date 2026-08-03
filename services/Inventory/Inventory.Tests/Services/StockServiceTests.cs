using FluentAssertions;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Moq;

namespace Inventory.Tests.Services;

public class StockServiceTests
{
    private readonly Mock<IStockItemRepository> _stockItemRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IWarehouseRepository> _warehouseRepositoryMock;
    private readonly Mock<IInventoryTransactionRepository> _transactionRepositoryMock;
    private readonly StockService _service;

    public StockServiceTests()
    {
        _stockItemRepositoryMock = new Mock<IStockItemRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _warehouseRepositoryMock = new Mock<IWarehouseRepository>();
        _transactionRepositoryMock = new Mock<IInventoryTransactionRepository>();
        _service = new StockService(
            _stockItemRepositoryMock.Object, 
            _transactionRepositoryMock.Object,
            _productRepositoryMock.Object, 
            _warehouseRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllStockItemsAsync_ReturnsAllItems()
    {
        // Arrange
        var stockItems = new List<StockItem>
        {
            new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), WarehouseId = Guid.NewGuid(), QuantityOnHand = 100 },
            new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), WarehouseId = Guid.NewGuid(), QuantityOnHand = 200 }
        };
        _stockItemRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(stockItems);

        // Act
        var result = await _service.GetAllStockItemsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStockByWarehouseAsync_ReturnsWarehouseStock()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var stockItems = new List<StockItem>
        {
            new() { Id = Guid.NewGuid(), WarehouseId = warehouseId, QuantityOnHand = 100, QuantityAllocated = 0 },
            new() { Id = Guid.NewGuid(), WarehouseId = warehouseId, QuantityOnHand = 150, QuantityAllocated = 0 }
        };
        _stockItemRepositoryMock.Setup(x => x.GetByWarehouseIdAsync(warehouseId)).ReturnsAsync(stockItems);

        // Act
        var result = await _service.GetStockByWarehouseAsync(warehouseId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStockSummaryAsync_GroupsByProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Test Product", SKU = "TEST001" };
        var stockItems = new List<StockItem>
        {
            new() { Id = Guid.NewGuid(), ProductId = productId, Product = product, QuantityOnHand = 50, QuantityAllocated = 10, AverageCost = 10m },
            new() { Id = Guid.NewGuid(), ProductId = productId, Product = product, QuantityOnHand = 30, QuantityAllocated = 5, AverageCost = 10m }
        };
        _stockItemRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(stockItems);

        // Act
        var result = await _service.GetStockSummaryAsync();

        // Assert
        result.Should().HaveCount(1);
        var summary = result.First();
        summary.TotalOnHand.Should().Be(80);
        summary.TotalAvailable.Should().Be(65); // 80 - 15 allocated
    }

    [Fact]
    public async Task GetRecentTransactionsAsync_ReturnsTransactions()
    {
        // Arrange
        var transactions = new List<InventoryTransaction>
        {
            new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), TransactionDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), TransactionDate = DateTime.UtcNow.AddDays(-1) }
        };
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetRecentTransactionsAsync(10);

        // Assert
        result.Should().HaveCount(2);
    }
}
