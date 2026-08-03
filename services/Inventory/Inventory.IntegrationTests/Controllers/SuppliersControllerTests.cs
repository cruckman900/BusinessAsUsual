using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Inventory.IntegrationTests.Controllers;

public class SuppliersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SuppliersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetSuppliers_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/inventory/suppliers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSuppliers_ReturnsSupplierList()
    {
        // Act
        var suppliers = await _client.GetFromJsonAsync<List<SupplierDto>>("/api/inventory/suppliers");

        // Assert
        suppliers.Should().NotBeNull();
        suppliers.Should().NotBeEmpty();
    }
}
