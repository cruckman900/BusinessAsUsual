namespace BusinessAsUsual.Core.Events;

/// <summary>
/// Base type for all integration events published across BAU modules.
/// Integration events describe something that has already happened
/// (past tense, e.g. OpportunityWon) and carry enough data for a
/// consumer to react without calling back into the source module.
/// </summary>
public abstract class IntegrationEvent
{
    /// <summary>Unique id for de-duplication / idempotency.</summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>UTC timestamp the event was created.</summary>
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Logical event name used for routing/serialization. Defaults to the
    /// runtime type name; override for a stable contract across services.
    /// </summary>
    public virtual string EventType => GetType().Name;
}
