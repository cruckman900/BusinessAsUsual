using System.Net;
using System.Net.Http.Json;
using Finance.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Finance.IntegrationTests.Controllers;

public class InvoicesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InvoicesControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllInvoices_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var invoices = await response.Content.ReadFromJsonAsync<List<InvoiceDto>>();
        invoices.Should().NotBeNull();
        invoices.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetInvoiceById_WithValidId_ShouldReturnOk()
    {
        // Arrange - Get all invoices first to get a valid ID
        var allResponse = await _client.GetAsync("/api/invoices");
        var invoices = await allResponse.Content.ReadFromJsonAsync<List<InvoiceDto>>();
        var firstInvoice = invoices!.First();

        // Act
        var response = await _client.GetAsync($"/api/invoices/{firstInvoice.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var invoice = await response.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice.Should().NotBeNull();
        invoice!.Id.Should().Be(firstInvoice.Id);
    }

    [Fact]
    public async Task GetInvoiceById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices/INVALID_ID");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateInvoice_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateInvoiceRequest
        {
            CustomerId = "CUST-001",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new()
                {
                    Description = "Test Service",
                    Quantity = 1,
                    UnitPrice = 100m
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoices", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var invoice = await response.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice.Should().NotBeNull();
        invoice!.InvoiceNumber.Should().NotBeNullOrEmpty();
        invoice.CustomerName.Should().Be("Test Customer");
    }

    [Fact]
    public async Task UpdateInvoice_WithValidId_ShouldReturnOk()
    {
        // Arrange - Get an existing invoice
        var allResponse = await _client.GetAsync("/api/invoices");
        var invoices = await allResponse.Content.ReadFromJsonAsync<List<InvoiceDto>>();
        var invoice = invoices!.First();

        var updateRequest = new UpdateInvoiceRequest
        {
            Notes = "Updated notes"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/invoices/{invoice.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendInvoice_WithValidId_ShouldReturnOk()
    {
        // Arrange - Create a new invoice first
        var createRequest = new CreateInvoiceRequest
        {
            CustomerId = "CUST-002",
            CustomerName = "Send Test Customer",
            CustomerEmail = "send@example.com",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new()
                {
                    Description = "Test Item",
                    Quantity = 1,
                    UnitPrice = 50m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoice = await createResponse.Content.ReadFromJsonAsync<InvoiceDto>();

        // Act
        var response = await _client.PostAsync($"/api/invoices/{invoice!.Id}/send", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedInvoice = await response.Content.ReadFromJsonAsync<InvoiceDto>();
        updatedInvoice!.Status.Should().Be("Sent");
    }

    [Fact]
    public async Task DeleteInvoice_WithValidId_ShouldReturnNoContent()
    {
        // Arrange - Create a new invoice to delete
        var createRequest = new CreateInvoiceRequest
        {
            CustomerId = "CUST-003",
            CustomerName = "Delete Test Customer",
            CustomerEmail = "delete@example.com",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new()
                {
                    Description = "Test Item",
                    Quantity = 1,
                    UnitPrice = 25m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoice = await createResponse.Content.ReadFromJsonAsync<InvoiceDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/invoices/{invoice!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
