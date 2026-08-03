namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by Sales when a quote is converted into an order.
/// CRM can track this conversion for opportunity/customer analysis.
/// </summary>
public sealed class QuoteConvertedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "sales.quote.converted";

    public string QuoteId { get; init; } = string.Empty;
    public string QuoteNumber { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
    public string OrderNumber { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "USD";
    public DateTime ConvertedDate { get; init; }
}
