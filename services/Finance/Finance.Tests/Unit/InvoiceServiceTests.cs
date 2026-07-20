using Finance.Application.DTOs;
using Finance.Application.Services;
using Finance.Domain.Enums;

namespace Finance.Tests.Unit;

public class InvoiceServiceTests
{
    private static MockInvoiceService NewService(out FinanceDataStore store)
    {
        store = new FinanceDataStore();
        return new MockInvoiceService(store);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllInvoicesAsync_ReturnsSeededData()
    {
        var service = NewService(out _);

        var invoices = (await service.GetAllInvoicesAsync()).ToList();

        Assert.NotEmpty(invoices);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateInvoiceAsync_ComputesTotalsAndDefaultsToDraft()
    {
        var service = NewService(out _);

        var invoice = await service.CreateInvoiceAsync(new CreateInvoiceRequest
        {
            CustomerName = "Test Co",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new() { Description = "Item", Quantity = 2, UnitPrice = 100m, TaxPercent = 10m }
            }
        });

        Assert.Equal(InvoiceStatus.Draft.ToString(), invoice.Status);
        Assert.Equal(200m, invoice.Subtotal);
        Assert.Equal(20m, invoice.TotalTax);
        Assert.Equal(220m, invoice.Total);
        Assert.Equal(220m, invoice.BalanceDue);
        Assert.StartsWith("INV-", invoice.InvoiceNumber);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SendInvoiceAsync_SetsStatusToSent()
    {
        var service = NewService(out var store);
        var draft = store.Invoices.First(i => i.Status == InvoiceStatus.Draft
            || i.Status == InvoiceStatus.Sent);

        var sent = await service.SendInvoiceAsync(draft.Id);

        Assert.Equal(InvoiceStatus.Sent.ToString(), sent.Status);
        Assert.NotNull(sent.SentDate);
    }
}
