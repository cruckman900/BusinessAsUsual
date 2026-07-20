namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by CRM when an opportunity is marked Closed-Won. Finance consumes
/// this to spin up a draft invoice for the customer.
///
/// This is a cross-module contract, so it lives in BusinessAsUsual.Core where
/// both the publisher (CRM) and the consumer (Finance) can reference it.
/// </summary>
public sealed class OpportunityWonIntegrationEvent : IntegrationEvent
{
    public override string EventType => "crm.opportunity.won";

    public string OpportunityId { get; init; } = string.Empty;
    public string OpportunityName { get; init; } = string.Empty;
    public string? CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public string? ProductCategory { get; init; }
    public string? ProductDescription { get; init; }
    public int? Quantity { get; init; }
    public string? AssignedToEmployeeId { get; init; }
}
