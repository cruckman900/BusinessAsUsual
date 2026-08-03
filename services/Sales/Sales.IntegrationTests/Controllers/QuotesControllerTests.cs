using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Sales.Application.DTOs;
using Sales.Domain.Enums;

namespace Sales.IntegrationTests.Controllers;

public class QuotesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public QuotesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllQuotes_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/sales/quotes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var quotes = await response.Content.ReadFromJsonAsync<IEnumerable<QuoteDto>>();
        quotes.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateQuote_ShouldReturnCreated()
    {
        // Arrange
        var createRequest = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            CustomerPhone = "555-1234",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            Notes = "Test quote",
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new()
                {
                    ProductId = "P1",
                    ProductName = "Test Product",
                    Quantity = 2,
                    UnitPrice = 50.00m,
                    DiscountPercentage = 10,
                    TaxPercentage = 8.5m
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales/quotes", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdQuote = await response.Content.ReadFromJsonAsync<QuoteDto>();
        createdQuote.Should().NotBeNull();
        createdQuote!.CustomerName.Should().Be("Test Customer");
        createdQuote.Status.Should().Be(QuoteStatus.Draft);
    }

    [Fact]
    public async Task GetQuoteById_WithValidId_ShouldReturnOk()
    {
        // Arrange - First create a quote
        var createRequest = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateQuoteLineItemDto>
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
        var createResponse = await _client.PostAsJsonAsync("/api/sales/quotes", createRequest);
        var createdQuote = await createResponse.Content.ReadFromJsonAsync<QuoteDto>();

        // Act
        var response = await _client.GetAsync($"/api/sales/quotes/{createdQuote!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var quote = await response.Content.ReadFromJsonAsync<QuoteDto>();
        quote.Should().NotBeNull();
        quote!.Id.Should().Be(createdQuote.Id);
    }

    [Fact]
    public async Task GetQuoteById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/sales/quotes/INVALID_ID");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateQuote_ShouldReturnOk()
    {
        // Arrange - First create a quote
        var createRequest = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Original Customer",
            CustomerEmail = "original@example.com",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateQuoteLineItemDto>
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
        var createResponse = await _client.PostAsJsonAsync("/api/sales/quotes", createRequest);
        var createdQuote = await createResponse.Content.ReadFromJsonAsync<QuoteDto>();

        var updateRequest = new UpdateQuoteDto
        {
            Id = createdQuote!.Id,
            CustomerId = "C1",
            CustomerName = "Updated Customer",
            CustomerEmail = "updated@example.com",
            ExpiryDate = DateTime.UtcNow.AddDays(60),
            Notes = "Updated notes",
            LineItems = new List<CreateQuoteLineItemDto>
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
        var response = await _client.PutAsJsonAsync($"/api/sales/quotes/{createdQuote.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedQuote = await response.Content.ReadFromJsonAsync<QuoteDto>();
        updatedQuote.Should().NotBeNull();
        updatedQuote!.CustomerName.Should().Be("Updated Customer");
    }

    [Fact]
    public async Task SendQuote_ShouldReturnOk()
    {
        // Arrange - Create a quote first
        var createRequest = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateQuoteLineItemDto>
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
        var createResponse = await _client.PostAsJsonAsync("/api/sales/quotes", createRequest);
        var createdQuote = await createResponse.Content.ReadFromJsonAsync<QuoteDto>();

        // Act
        var response = await _client.PostAsync($"/api/sales/quotes/{createdQuote!.Id}/send", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sentQuote = await response.Content.ReadFromJsonAsync<QuoteDto>();
        sentQuote.Should().NotBeNull();
        sentQuote!.Status.Should().Be(QuoteStatus.Sent);
        sentQuote.SentDate.Should().NotBeNull();
    }

    [Fact]
    public async Task AcceptQuote_ShouldReturnOk()
    {
        // Arrange - Create and send a quote first
        var createRequest = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateQuoteLineItemDto>
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
        var createResponse = await _client.PostAsJsonAsync("/api/sales/quotes", createRequest);
        var createdQuote = await createResponse.Content.ReadFromJsonAsync<QuoteDto>();
        await _client.PostAsync($"/api/sales/quotes/{createdQuote!.Id}/send", null);

        // Act
        var response = await _client.PostAsync($"/api/sales/quotes/{createdQuote.Id}/accept", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var acceptedQuote = await response.Content.ReadFromJsonAsync<QuoteDto>();
        acceptedQuote.Should().NotBeNull();
        acceptedQuote!.Status.Should().Be(QuoteStatus.Accepted);
        acceptedQuote.AcceptedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ConvertQuoteToOrder_ShouldReturnOk()
    {
        // Arrange - Create, send, and accept a quote first
        var createRequest = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateQuoteLineItemDto>
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
        var createResponse = await _client.PostAsJsonAsync("/api/sales/quotes", createRequest);
        var createdQuote = await createResponse.Content.ReadFromJsonAsync<QuoteDto>();
        await _client.PostAsync($"/api/sales/quotes/{createdQuote!.Id}/send", null);
        await _client.PostAsync($"/api/sales/quotes/{createdQuote.Id}/accept", null);

        // Act
        var response = await _client.PostAsync($"/api/sales/quotes/{createdQuote.Id}/convert", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Status.Should().Be(OrderStatus.Pending);
        order.CustomerId.Should().Be(createdQuote.CustomerId);
    }

    [Fact]
    public async Task DeleteQuote_ShouldReturnNoContent()
    {
        // Arrange - Create a quote first
        var createRequest = new CreateQuoteDto
        {
            CustomerId = "C1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateQuoteLineItemDto>
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
        var createResponse = await _client.PostAsJsonAsync("/api/sales/quotes", createRequest);
        var createdQuote = await createResponse.Content.ReadFromJsonAsync<QuoteDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/sales/quotes/{createdQuote!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/sales/quotes/{createdQuote.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
