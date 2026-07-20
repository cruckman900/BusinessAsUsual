using Finance.Application.DTOs;
using Finance.Application.Services;
using Finance.Domain.Enums;

namespace Finance.Tests.Unit;

public class PaymentServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task RecordPaymentAsync_MarksInvoicePaid_WhenBalanceCleared()
    {
        var store = new FinanceDataStore();
        var invoiceService = new MockInvoiceService(store);
        var paymentService = new MockPaymentService(store);

        var invoice = await invoiceService.CreateInvoiceAsync(new CreateInvoiceRequest
        {
            CustomerName = "Payer Co",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new() { Description = "Service", Quantity = 1, UnitPrice = 500m }
            }
        });

        await paymentService.RecordPaymentAsync(new RecordPaymentRequest
        {
            InvoiceId = invoice.Id,
            Amount = 500m,
            Method = PaymentMethod.BankTransfer,
            Status = PaymentStatus.Completed
        });

        var updated = await invoiceService.GetInvoiceByIdAsync(invoice.Id);
        Assert.NotNull(updated);
        Assert.Equal(InvoiceStatus.Paid.ToString(), updated!.Status);
        Assert.Equal(0m, updated.BalanceDue);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RecordPaymentAsync_MarksPartiallyPaid_WhenBalanceRemains()
    {
        var store = new FinanceDataStore();
        var invoiceService = new MockInvoiceService(store);
        var paymentService = new MockPaymentService(store);

        var invoice = await invoiceService.CreateInvoiceAsync(new CreateInvoiceRequest
        {
            CustomerName = "Partial Co",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new() { Description = "Service", Quantity = 1, UnitPrice = 1000m }
            }
        });

        await paymentService.RecordPaymentAsync(new RecordPaymentRequest
        {
            InvoiceId = invoice.Id,
            Amount = 400m,
            Status = PaymentStatus.Completed
        });

        var updated = await invoiceService.GetInvoiceByIdAsync(invoice.Id);
        Assert.Equal(InvoiceStatus.PartiallyPaid.ToString(), updated!.Status);
        Assert.Equal(600m, updated.BalanceDue);
    }
}
