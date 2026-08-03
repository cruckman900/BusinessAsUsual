using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Inventory.IntegrationTests.Controllers;

public class StockControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public StockControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetStock_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/inventory/stock/items");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStock_ReturnsStockList()
    {
        // Act
        var stockItems = await _client.GetFromJsonAsync<List<StockItemDto>>("/api/inventory/stock/items");

        // Assert
        stockItems.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStockSummary_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/inventory/stock/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStockSummary_ReturnsSummaryList()
    {
        // Act
        var summary = await _client.GetFromJsonAsync<List<StockSummaryDto>>("/api/inventory/stock/summary");

        // Assert
        summary.Should().NotBeNull();
    }
}
