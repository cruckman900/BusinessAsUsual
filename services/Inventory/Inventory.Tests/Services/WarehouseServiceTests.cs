using FluentAssertions;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Moq;

namespace Inventory.Tests.Services;

public class WarehouseServiceTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepositoryMock;
    private readonly Mock<IStockItemRepository> _stockItemRepositoryMock;
    private readonly WarehouseService _service;

    public WarehouseServiceTests()
    {
        _warehouseRepositoryMock = new Mock<IWarehouseRepository>();
        _stockItemRepositoryMock = new Mock<IStockItemRepository>();
        _service = new WarehouseService(_warehouseRepositoryMock.Object, _stockItemRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllWarehousesAsync_ReturnsAllWarehouses()
    {
        // Arrange
        var warehouses = new List<Warehouse>
        {
            new() { Id = Guid.NewGuid(), Name = "Main Warehouse", Code = "WH-001", IsActive = true, BinLocations = new List<BinLocation>() },
            new() { Id = Guid.NewGuid(), Name = "Secondary Warehouse", Code = "WH-002", IsActive = true, BinLocations = new List<BinLocation>() }
        };
        _warehouseRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(warehouses);
        _stockItemRepositoryMock.Setup(x => x.GetByWarehouseIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<StockItem>());

        // Act
        var result = await _service.GetAllWarehousesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWarehouseByIdAsync_WithValidId_ReturnsWarehouse()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse 
        { 
            Id = warehouseId, 
            Name = "Test Warehouse", 
            Code = "TEST-001",
            IsActive = true 
        };
        _warehouseRepositoryMock.Setup(x => x.GetByIdAsync(warehouseId)).ReturnsAsync(warehouse);

        // Act
        var result = await _service.GetWarehouseByIdAsync(warehouseId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(warehouseId);
        result.Name.Should().Be("Test Warehouse");
    }

    [Fact]
    public async Task GetWarehouseByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        _warehouseRepositoryMock.Setup(x => x.GetByIdAsync(warehouseId)).ReturnsAsync((Warehouse?)null);

        // Act
        var result = await _service.GetWarehouseByIdAsync(warehouseId);

        // Assert
        result.Should().BeNull();
    }
}
