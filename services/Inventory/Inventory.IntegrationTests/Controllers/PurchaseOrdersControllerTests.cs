using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Inventory.IntegrationTests.Controllers;

public class PurchaseOrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PurchaseOrdersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPurchaseOrders_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/inventory/purchaseorders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPurchaseOrders_ReturnsList()
    {
        // Act
        var orders = await _client.GetFromJsonAsync<List<PurchaseOrderDto>>("/api/inventory/purchaseorders");

        // Assert
        orders.Should().NotBeNull();
        orders.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPurchaseOrderById_WithValidId_ReturnsOrder()
    {
        // Arrange - Get first PO
        var orders = await _client.GetFromJsonAsync<List<PurchaseOrderDto>>("/api/inventory/purchaseorders");
        var validId = orders!.First().Id;

        // Act
        var order = await _client.GetFromJsonAsync<PurchaseOrderDto>($"/api/inventory/purchaseorders/{validId}");

        // Assert
        order.Should().NotBeNull();
        order!.Id.Should().Be(validId);
    }

    [Fact]
    public async Task GetPurchaseOrderById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/inventory/purchaseorders/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
