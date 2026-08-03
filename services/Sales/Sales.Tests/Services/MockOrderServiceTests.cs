using FluentAssertions;
using Moq;
using Sales.Application.DTOs;
using Sales.Application.Services;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;

namespace Sales.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _eventBusMock = new Mock<IEventBus>();
        _service = new OrderService(_repositoryMock.Object, _eventBusMock.Object);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = "1", OrderNumber = "ORD-001", CustomerName = "Test Customer", LineItems = new(), Payments = new() },
            new() { Id = "2", OrderNumber = "ORD-002", CustomerName = "Another Customer", LineItems = new(), Payments = new() }
        };
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _service.GetAllOrdersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<OrderDto>();
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithValidId_ShouldReturnOrder()
    {
        // Arrange
        var order = new Order 
        { 
            Id = "1", 
            OrderNumber = "ORD-001", 
            CustomerName = "Test Customer",
            LineItems = new(),
            Payments = new()
        };
        _repositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(order);

        // Act
        var result = await _service.GetOrderByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.OrderNumber.Should().Be("ORD-001");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Order?)null);

        // Act
        var result = await _service.GetOrderByIdAsync("INVALID_ID");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrderAndPublishEvent()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            CustomerPhone = "555-1234",
            ShippingMethod = ShippingMethod.Standard,
            ShippingAddressLine1 = "123 Test St",
            ShippingCity = "Test City",
            ShippingState = "TS",
            ShippingPostalCode = "12345",
            ShippingCountry = "USA",
            ShippingCost = 10.00m,
            LineItems = new List<CreateOrderLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 2,
                    UnitPrice = 50.00m,
                    DiscountPercentage = 0,
                    TaxPercentage = 8.5m
                }
            }
        };

        _repositoryMock.Setup(x => x.CountAsync()).ReturnsAsync(5);
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _service.CreateOrderAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.OrderNumber.Should().StartWith("O-");
        result.CustomerId.Should().Be(request.CustomerId);
        result.CustomerName.Should().Be(request.CustomerName);
        result.Status.Should().Be(OrderStatus.Draft);
        result.LineItems.Should().HaveCount(1);
        result.Total.Should().BeGreaterThan(0);

        // Verify event was published
        _eventBusMock.Verify(
            x => x.PublishAsync(It.Is<OrderCreatedIntegrationEvent>(
                e => e.OrderId == result.Id &&
                     e.CustomerId == request.CustomerId
            ), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateOrderAsync_WithValidId_ShouldUpdateOrder()
    {
        // Arrange
        var existingOrder = new Order
        {
            Id = "1",
            OrderNumber = "ORD-001",
            CustomerId = "C1",
            CustomerName = "Old Name",
            LineItems = new(),
            Payments = new()
        };

        var updateRequest = new UpdateOrderDto
        {
            Id = "1",
            CustomerId = "C1",
            CustomerName = "Updated Customer Name",
            CustomerEmail = "updated@test.com",
            ShippingMethod = ShippingMethod.Express,
            LineItems = new List<CreateOrderLineItemDto>()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(existingOrder);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _service.UpdateOrderAsync(updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Updated Customer Name");
        result.ShippingMethod.Should().Be(ShippingMethod.Express);
    }

    [Fact]
    public async Task UpdateOrderAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var updateRequest = new UpdateOrderDto
        {
            Id = "INVALID_ID",
            CustomerId = "C1",
            CustomerName = "Test",
            CustomerEmail = "test@test.com",
            ShippingMethod = ShippingMethod.Standard,
            LineItems = new List<CreateOrderLineItemDto>()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateOrderAsync(updateRequest)
        );
    }

    [Fact]
    public async Task ConfirmOrderAsync_ShouldChangeStatusAndPublishEvent()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            OrderNumber = "ORD-001",
            CustomerId = "C1",
            CustomerName = "Test Customer",
            Status = OrderStatus.Pending,
            LineItems = new(),
            Payments = new()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(order);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _service.ConfirmOrderAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Confirmed);
        result.ConfirmedDate.Should().NotBeNull();

        // Verify event was published
        _eventBusMock.Verify(
            x => x.PublishAsync(It.IsAny<OrderConfirmedIntegrationEvent>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ShipOrderAsync_ShouldChangeStatusAndPublishEvent()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            OrderNumber = "ORD-001",
            CustomerId = "C1",
            CustomerName = "Test Customer",
            Status = OrderStatus.Confirmed,
            LineItems = new(),
            Payments = new()
        };
        var trackingNumber = "TRACK123456";

        _repositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(order);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _service.ShipOrderAsync("1", trackingNumber);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Shipped);
        result.ShippedDate.Should().NotBeNull();
        result.TrackingNumber.Should().Be(trackingNumber);

        // Verify event was published
        _eventBusMock.Verify(
            x => x.PublishAsync(It.Is<OrderShippedIntegrationEvent>(
                e => e.OrderId == result.Id &&
                     e.TrackingNumber == trackingNumber
            ), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeliverOrderAsync_ShouldChangeStatus()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            OrderNumber = "ORD-001",
            Status = OrderStatus.Shipped,
            LineItems = new(),
            Payments = new()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(order);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _service.DeliverOrderAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Delivered);
        result.DeliveredDate.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldChangeStatus()
    {
        // Arrange
        var order = new Order
        {
            Id = "1",
            OrderNumber = "ORD-001",
            Status = OrderStatus.Pending,
            LineItems = new(),
            Payments = new()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync("1")).ReturnsAsync(order);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _service.CancelOrderAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task DeleteOrderAsync_ShouldCallRepository()
    {
        // Arrange
        _repositoryMock.Setup(x => x.DeleteAsync("1")).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteOrderAsync("1");

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync("1"), Times.Once);
    }
}
