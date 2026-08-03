namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by Sales when a new order is created from a quote or directly.
/// Other modules can react to track customer activity or prepare for fulfillment.
/// </summary>
public sealed class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "sales.order.created";

    public string OrderId { get; init; } = string.Empty;
    public string OrderNumber { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "USD";
    public DateTime OrderDate { get; init; }

    /// <summary>Line items with product details for downstream processing.</summary>
    public List<OrderLineItemDto> LineItems { get; init; } = new();
}

public sealed class OrderLineItemDto
{
    public string ProductId { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string? SKU { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
