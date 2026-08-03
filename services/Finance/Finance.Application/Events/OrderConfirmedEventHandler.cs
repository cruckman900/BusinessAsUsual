using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Finance.Application.DTOs;
using Finance.Application.Services;
using Finance.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Finance.Application.Events;

/// <summary>
/// When Sales confirms an order (payment received), create a matching draft invoice
/// in Finance. The invoice is linked back to the originating sales order via
/// SourceModule/SourceReferenceId for traceability.
/// </summary>
public sealed class OrderConfirmedEventHandler : IIntegrationEventHandler<OrderConfirmedIntegrationEvent>
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<OrderConfirmedEventHandler> _logger;

    public OrderConfirmedEventHandler(IInvoiceService invoiceService, ILogger<OrderConfirmedEventHandler> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderConfirmedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "💰 Processing OrderConfirmed event for Order {OrderNumber} - creating invoice",
            @event.OrderNumber);

        try
        {
            // Calculate total from line items
            decimal subtotal = @event.LineItems.Sum(li => li.Quantity * li.UnitPrice);

            var request = new CreateInvoiceRequest
            {
                CustomerId = @event.CustomerId,
                CustomerName = @event.CustomerName,
                Currency = Currency.USD, // Could parse from event if needed
                DueDate = DateTime.UtcNow.AddDays(30), // Net 30 terms
                SourceModule = "sales",
                SourceReferenceId = @event.OrderId,
                Notes = $"Auto-generated from confirmed sales order {@event.OrderNumber} on {@event.ConfirmedDate:yyyy-MM-dd}.",
                LineItems = @event.LineItems.Select(li => new CreateInvoiceLineItemRequest
                {
                    Description = $"{li.ProductName} (SKU: {li.SKU ?? "N/A"})",
                    Quantity = (int)li.Quantity,
                    UnitPrice = li.UnitPrice,
                    ProductCategory = null // Could be enriched from Inventory if needed
                }).ToList()
            };

            var invoice = await _invoiceService.CreateInvoiceAsync(request);

            _logger.LogInformation(
                "✅ Created draft invoice {InvoiceNumber} from confirmed order {OrderNumber} (Total: ${Total:N2})",
                invoice.InvoiceNumber, @event.OrderNumber, subtotal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Failed to create invoice for order {OrderNumber}",
                @event.OrderNumber);

            // Don't throw - event handler failures shouldn't break the order confirmation
            // The invoice can be created manually if the auto-creation fails
        }
    }
}
