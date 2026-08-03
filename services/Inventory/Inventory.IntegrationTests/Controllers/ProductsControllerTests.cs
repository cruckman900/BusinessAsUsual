using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Inventory.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/inventory/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducts_ReturnsProductList()
    {
        // Act
        var products = await _client.GetFromJsonAsync<List<ProductDto>>("/api/inventory/products");

        // Assert
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsProduct()
    {
        // Arrange - First get all products to find a valid ID
        var products = await _client.GetFromJsonAsync<List<ProductDto>>("/api/inventory/products");
        var validId = products!.First().Id;

        // Act
        var product = await _client.GetFromJsonAsync<ProductDto>($"/api/inventory/products/{validId}");

        // Assert
        product.Should().NotBeNull();
        product!.Id.Should().Be(validId);
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/inventory/products/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Integration Test Product",
            SKU = $"TEST-{Guid.NewGuid().ToString()[..8]}",
            Price = 99.99m,
            Cost = 50.00m,
            Category = "Test Category",
            UnitOfMeasure = "Each",
            ReorderPoint = 10,
            ReorderQuantity = 50
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/inventory/products", createDto);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be(createDto.Name);
        createdProduct.SKU.Should().Be(createDto.SKU);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsUpdatedProduct()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "Product to Update",
            SKU = $"UPD-{Guid.NewGuid().ToString()[..8]}",
            Price = 50.00m,
            Cost = 25.00m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/inventory/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateDto = new UpdateProductDto
        {
            Id = createdProduct!.Id,
            Name = "Updated Product Name",
            SKU = createdProduct.SKU,
            Price = 75.00m,
            Cost = 30.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/inventory/products/{createdProduct.Id}", updateDto);
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Updated Product Name");
        updatedProduct.Price.Should().Be(75.00m);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "Product to Delete",
            SKU = $"DEL-{Guid.NewGuid().ToString()[..8]}",
            Price = 10.00m,
            Cost = 5.00m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/inventory/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/inventory/products/{createdProduct!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
