namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by Sales when an order is confirmed (payment received, ready for fulfillment).
/// Inventory should reserve/allocate stock when this event is received.
/// </summary>
public sealed class OrderConfirmedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "sales.order.confirmed";

    public string OrderId { get; init; } = string.Empty;
    public string OrderNumber { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public DateTime ConfirmedDate { get; init; }

    /// <summary>Products to reserve in inventory.</summary>
    public List<OrderLineItemDto> LineItems { get; init; } = new();
}

