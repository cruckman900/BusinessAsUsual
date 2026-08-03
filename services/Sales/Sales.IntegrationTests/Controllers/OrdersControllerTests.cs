using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Sales.Application.DTOs;
using Sales.Domain.Enums;

namespace Sales.IntegrationTests.Controllers;

public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrdersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllOrders_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/sales/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
        orders.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreated()
    {
        // Arrange
        var createRequest = new CreateOrderDto
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

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales/orders", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        createdOrder.Should().NotBeNull();
        createdOrder!.CustomerName.Should().Be("Test Customer");
        createdOrder.Status.Should().Be(OrderStatus.Draft);
    }

    [Fact]
    public async Task GetOrderById_WithValidId_ShouldReturnOk()
    {
        // Arrange - First create an order
        var createRequest = new CreateOrderDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ShippingMethod = ShippingMethod.Standard,
            LineItems = new List<CreateOrderLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 1,
                    UnitPrice = 100.00m
                }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/sales/orders", createRequest);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        // Act
        var response = await _client.GetAsync($"/api/sales/orders/{createdOrder!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Id.Should().Be(createdOrder.Id);
    }

    [Fact]
    public async Task GetOrderById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/sales/orders/INVALID_ID");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnOk()
    {
        // Arrange - First create an order
        var createRequest = new CreateOrderDto
        {
            CustomerId = "C1",
            CustomerName = "Original Customer",
            CustomerEmail = "original@example.com",
            ShippingMethod = ShippingMethod.Standard,
            LineItems = new List<CreateOrderLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 1,
                    UnitPrice = 100.00m
                }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/sales/orders", createRequest);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        var updateRequest = new UpdateOrderDto
        {
            Id = createdOrder!.Id,
            CustomerId = "C1",
            CustomerName = "Updated Customer",
            CustomerEmail = "updated@example.com",
            ShippingMethod = ShippingMethod.Express,
            LineItems = new List<CreateOrderLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 2,
                    UnitPrice = 100.00m
                }
            }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/sales/orders/{createdOrder.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        updatedOrder.Should().NotBeNull();
        updatedOrder!.CustomerName.Should().Be("Updated Customer");
    }

    [Fact]
    public async Task ConfirmOrder_ShouldReturnOk()
    {
        // Arrange - Create an order first
        var createRequest = new CreateOrderDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ShippingMethod = ShippingMethod.Standard,
            LineItems = new List<CreateOrderLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 1,
                    UnitPrice = 100.00m
                }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/sales/orders", createRequest);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        // Act
        var response = await _client.PostAsync($"/api/sales/orders/{createdOrder!.Id}/confirm", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var confirmedOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        confirmedOrder.Should().NotBeNull();
        confirmedOrder!.Status.Should().Be(OrderStatus.Confirmed);
        confirmedOrder.ConfirmedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnNoContent()
    {
        // Arrange - Create an order first
        var createRequest = new CreateOrderDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ShippingMethod = ShippingMethod.Standard,
            LineItems = new List<CreateOrderLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 1,
                    UnitPrice = 100.00m
                }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/sales/orders", createRequest);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/sales/orders/{createdOrder!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/sales/orders/{createdOrder.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
