using System.Net;
using System.Net.Http.Json;
using Finance.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Finance.IntegrationTests.Controllers;

public class BillsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BillsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllBills_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/bills");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var bills = await response.Content.ReadFromJsonAsync<List<BillDto>>();
        bills.Should().NotBeNull();
        bills.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetBillById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var allResponse = await _client.GetAsync("/api/bills");
        var bills = await allResponse.Content.ReadFromJsonAsync<List<BillDto>>();
        var firstBill = bills!.First();

        // Act
        var response = await _client.GetAsync($"/api/bills/{firstBill.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var bill = await response.Content.ReadFromJsonAsync<BillDto>();
        bill.Should().NotBeNull();
        bill!.Id.Should().Be(firstBill.Id);
    }

    [Fact]
    public async Task CreateBill_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateBillRequest
        {
            VendorName = "Integration Test Vendor",
            DueDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateBillLineItemRequest>
            {
                new()
                {
                    Description = "Test Expense",
                    Quantity = 1,
                    UnitPrice = 150m
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/bills", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var bill = await response.Content.ReadFromJsonAsync<BillDto>();
        bill.Should().NotBeNull();
        bill!.BillNumber.Should().NotBeNullOrEmpty();
        bill.VendorName.Should().Be("Integration Test Vendor");
    }

    [Fact]
    public async Task ApproveBill_WithValidId_ShouldReturnOk()
    {
        // Arrange - Create a bill first
        var createRequest = new CreateBillRequest
        {
            VendorName = "Approve Test Vendor",
            DueDate = DateTime.UtcNow.AddDays(15),
            LineItems = new List<CreateBillLineItemRequest>
            {
                new()
                {
                    Description = "Approvable Item",
                    Quantity = 2,
                    UnitPrice = 75m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/bills", createRequest);
        var bill = await createResponse.Content.ReadFromJsonAsync<BillDto>();

        // Act
        var response = await _client.PostAsync($"/api/bills/{bill!.Id}/approve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteBill_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var createRequest = new CreateBillRequest
        {
            VendorName = "Delete Test Vendor",
            DueDate = DateTime.UtcNow.AddDays(20),
            LineItems = new List<CreateBillLineItemRequest>
            {
                new()
                {
                    Description = "Deletable Item",
                    Quantity = 1,
                    UnitPrice = 50m
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/bills", createRequest);
        var bill = await createResponse.Content.ReadFromJsonAsync<BillDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/bills/{bill!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
