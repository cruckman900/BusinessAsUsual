namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by Sales when an order is shipped to the customer.
/// Inventory should decrement stock quantities when this event is received.
/// </summary>
public sealed class OrderShippedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "sales.order.shipped";

    public string OrderId { get; init; } = string.Empty;
    public string OrderNumber { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public DateTime ShippedDate { get; init; }
    public string? TrackingNumber { get; init; }
    public string ShippingMethod { get; init; } = string.Empty;

    /// <summary>Products to decrement from inventory stock.</summary>
    public List<OrderLineItemDto> LineItems { get; init; } = new();
}

