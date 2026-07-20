namespace BusinessAsUsual.Core.Events;

/// <summary>
/// Handles a specific integration event type. Register one or more handlers
/// per event; the event bus dispatches to all of them.
/// </summary>
public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
