using System.Net;
using System.Net.Http.Json;
using Finance.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Finance.IntegrationTests.Controllers;

public class PaymentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PaymentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllPayments_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/payments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payments = await response.Content.ReadFromJsonAsync<List<PaymentDto>>();
        payments.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPaymentById_WithValidId_ShouldReturnOk()
    {
        // Arrange - Get all payments to find a valid ID
        var allResponse = await _client.GetAsync("/api/payments");
        var payments = await allResponse.Content.ReadFromJsonAsync<List<PaymentDto>>();

        if (payments?.Any() == true)
        {
            var firstPayment = payments.First();

            // Act
            var response = await _client.GetAsync($"/api/payments/{firstPayment.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var payment = await response.Content.ReadFromJsonAsync<PaymentDto>();
            payment.Should().NotBeNull();
            payment!.Id.Should().Be(firstPayment.Id);
        }
    }

    [Fact]
    public async Task CreatePayment_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange - First get an invoice to pay
        var invoicesResponse = await _client.GetAsync("/api/invoices");
        var invoices = await invoicesResponse.Content.ReadFromJsonAsync<List<InvoiceDto>>();
        var invoice = invoices!.First();

        var request = new RecordPaymentRequest
        {
            InvoiceId = invoice.Id,
            Amount = 50m,
            Method = Finance.Domain.Enums.PaymentMethod.Check,
            PaymentDate = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payment = await response.Content.ReadFromJsonAsync<PaymentDto>();
        payment.Should().NotBeNull();
        payment!.Amount.Should().Be(50m);
    }
}
