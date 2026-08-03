using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Inventory.IntegrationTests.Controllers;

public class WarehousesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WarehousesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetWarehouses_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/inventory/warehouses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWarehouses_ReturnsWarehouseList()
    {
        // Act
        var warehouses = await _client.GetFromJsonAsync<List<WarehouseDto>>("/api/inventory/warehouses");

        // Assert
        warehouses.Should().NotBeNull();
        warehouses.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetWarehouseById_WithValidId_ReturnsWarehouse()
    {
        // Arrange - Get first warehouse
        var warehouses = await _client.GetFromJsonAsync<List<WarehouseDto>>("/api/inventory/warehouses");
        var validId = warehouses!.First().Id;

        // Act
        var warehouse = await _client.GetFromJsonAsync<WarehouseDto>($"/api/inventory/warehouses/{validId}");

        // Assert
        warehouse.Should().NotBeNull();
        warehouse!.Id.Should().Be(validId);
    }

    [Fact]
    public async Task GetWarehouseById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/inventory/warehouses/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
