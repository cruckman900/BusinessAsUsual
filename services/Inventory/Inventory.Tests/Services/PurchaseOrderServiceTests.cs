using FluentAssertions;
using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Moq;

namespace Inventory.Tests.Services;

public class PurchaseOrderServiceTests
{
    private readonly Mock<IPurchaseOrderRepository> _purchaseOrderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
    private readonly Mock<IStockItemRepository> _stockItemRepositoryMock;
    private readonly Mock<IWarehouseRepository> _warehouseRepositoryMock;
    private readonly Mock<IInventoryTransactionRepository> _transactionRepositoryMock;
    private readonly PurchaseOrderService _service;

    public PurchaseOrderServiceTests()
    {
        _purchaseOrderRepositoryMock = new Mock<IPurchaseOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _supplierRepositoryMock = new Mock<ISupplierRepository>();
        _stockItemRepositoryMock = new Mock<IStockItemRepository>();
        _warehouseRepositoryMock = new Mock<IWarehouseRepository>();
        _transactionRepositoryMock = new Mock<IInventoryTransactionRepository>();

        _service = new PurchaseOrderService(
            _purchaseOrderRepositoryMock.Object,
            _supplierRepositoryMock.Object,
            _warehouseRepositoryMock.Object,
            _productRepositoryMock.Object,
            _stockItemRepositoryMock.Object,
            _transactionRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GetAllPurchaseOrdersAsync_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<PurchaseOrder>
        {
            new() { Id = Guid.NewGuid(), OrderNumber = "PO-001", Status = PurchaseOrderStatus.Draft, Total = 100m },
            new() { Id = Guid.NewGuid(), OrderNumber = "PO-002", Status = PurchaseOrderStatus.Approved, Total = 200m }
        };
        _purchaseOrderRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _service.GetAllPurchaseOrdersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPurchaseOrderByIdAsync_WithValidId_ReturnsOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new PurchaseOrder 
        { 
            Id = orderId, 
            OrderNumber = "PO-123", 
            Status = PurchaseOrderStatus.Draft,
            Total = 500m,
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(7)
        };
        _purchaseOrderRepositoryMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _service.GetPurchaseOrderByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.OrderNumber.Should().Be("PO-123");
    }

    [Fact]
    public async Task GetPurchaseOrderByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _purchaseOrderRepositoryMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync((PurchaseOrder?)null);

        // Act
        var result = await _service.GetPurchaseOrderByIdAsync(orderId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPurchaseOrdersBySupplierAsync_ReturnsSupplierOrders()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var orders = new List<PurchaseOrder>
        {
            new() { Id = Guid.NewGuid(), OrderNumber = "PO-001", SupplierId = supplierId, Total = 100m },
            new() { Id = Guid.NewGuid(), OrderNumber = "PO-002", SupplierId = supplierId, Total = 200m }
        };

        _purchaseOrderRepositoryMock.Setup(x => x.GetBySupplierIdAsync(supplierId)).ReturnsAsync(orders);

        // Act
        var result = await _service.GetPurchaseOrdersBySupplierAsync(supplierId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(po => po.SupplierId == supplierId);
    }
}
