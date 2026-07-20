using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Finance.Application.DTOs;
using Finance.Application.Services;
using Finance.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Finance.Application.Events;

/// <summary>
/// When CRM reports an opportunity as won, create a matching draft invoice so
/// Finance can review, adjust, and send it. The invoice is linked back to the
/// originating opportunity via SourceModule/SourceReferenceId.
/// </summary>
public sealed class OpportunityWonHandler : IIntegrationEventHandler<OpportunityWonIntegrationEvent>
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<OpportunityWonHandler> _logger;

    public OpportunityWonHandler(IInvoiceService invoiceService, ILogger<OpportunityWonHandler> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    public async Task HandleAsync(OpportunityWonIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        var currency = Enum.TryParse<Currency>(@event.Currency, ignoreCase: true, out var parsed)
            ? parsed
            : Currency.USD;

        var request = new CreateInvoiceRequest
        {
            CustomerId = @event.CustomerId,
            CustomerName = @event.CustomerName,
            Currency = currency,
            DueDate = DateTime.UtcNow.AddDays(30),
            SourceModule = "crm",
            SourceReferenceId = @event.OpportunityId,
            Notes = $"Auto-generated from won opportunity '{@event.OpportunityName}'.",
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new()
                {
                    Description = @event.ProductDescription ?? @event.OpportunityName,
                    Quantity = @event.Quantity is > 0 ? @event.Quantity!.Value : 1,
                    UnitPrice = @event.Quantity is > 0 ? @event.Amount / @event.Quantity!.Value : @event.Amount,
                    ProductCategory = @event.ProductCategory
                }
            }
        };

        var invoice = await _invoiceService.CreateInvoiceAsync(request);
        _logger.LogInformation(
            "Created draft invoice {InvoiceNumber} from won opportunity {OpportunityId}",
            invoice.InvoiceNumber, @event.OpportunityId);
    }
}
