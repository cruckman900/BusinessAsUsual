using System.Net;
using System.Net.Http.Json;
using Finance.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Finance.Tests.Functional;

public class InvoicesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public InvoicesApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetInvoices_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/invoices");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var invoices = await response.Content.ReadFromJsonAsync<List<InvoiceDto>>();
        Assert.NotNull(invoices);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetMobileUiSpec_ReturnsFinanceSpec()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/finance/mobile/ui-spec");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("invoice-list", body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task CreateInvoice_ReturnsCreated()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/invoices", new CreateInvoiceRequest
        {
            CustomerName = "Functional Co",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new() { Description = "Test", Quantity = 1, UnitPrice = 99m }
            }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
