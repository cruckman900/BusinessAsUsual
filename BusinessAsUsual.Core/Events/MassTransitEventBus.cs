using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BusinessAsUsual.Core.Events;

/// <summary>
/// Broker-backed <see cref="IEventBus"/> that publishes integration events to
/// RabbitMQ (locally, or Amazon MQ for RabbitMQ on AWS) via MassTransit.
///
/// Publishers (<c>IEventBus.PublishAsync</c>) and consumers
/// (<see cref="IIntegrationEventHandler{TEvent}"/>) are unchanged: the actual
/// transport, exponential-backoff retry and dead-letter (<c>*_error</c>) queue
/// behaviour are configured by MassTransit at registration time.
/// </summary>
public sealed class MassTransitEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventBus(IPublishEndpoint publishEndpoint) =>
        _publishEndpoint = publishEndpoint;

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(@event);

        // Publish by the concrete runtime type so MassTransit routes to the
        // exchange/queue that the matching IntegrationEventConsumer<TEvent> owns.
        await _publishEndpoint.Publish(@event, @event.GetType(), cancellationToken);
    }
}

/// <summary>
/// Generic MassTransit consumer that bridges the broker to the existing
/// <see cref="IIntegrationEventHandler{TEvent}"/> pattern. Every registered
/// handler for <typeparamref name="TEvent"/> is resolved from a fresh DI scope
/// and invoked. Exceptions bubble up so MassTransit's retry / dead-letter
/// pipeline can take over.
/// </summary>
public sealed class IntegrationEventConsumer<TEvent> : IConsumer<TEvent>
    where TEvent : IntegrationEvent
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IntegrationEventConsumer<TEvent>> _logger;

    public IntegrationEventConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<IntegrationEventConsumer<TEvent>> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        using var scope = _scopeFactory.CreateScope();
        var handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler<TEvent>>()
            .ToList();

        if (handlers.Count == 0)
        {
            _logger.LogDebug("No handlers registered for integration event {EventType}", context.Message.EventType);
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(context.Message, context.CancellationToken);
        }
    }
}
