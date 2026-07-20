namespace BusinessAsUsual.Core.Events;

/// <summary>
/// Publishes integration events to interested consumers.
///
/// v1 is an in-process implementation (see <see cref="InProcessEventBus"/>).
/// The interface is intentionally transport-agnostic so a real broker
/// (RabbitMQ, Azure Service Bus, MassTransit, etc.) can be swapped in later
/// without changing any publisher or handler code.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publish an integration event. With the in-process bus the event is
    /// queued and dispatched to handlers on a background worker so the caller
    /// is not blocked by consumer work.
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}
