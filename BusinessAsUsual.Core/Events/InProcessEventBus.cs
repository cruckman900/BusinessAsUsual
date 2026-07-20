using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BusinessAsUsual.Core.Events;

/// <summary>
/// In-process, channel-backed event bus. Publishing enqueues the event and
/// returns immediately; a background worker (<see cref="EventBusDispatcher"/>)
/// drains the channel and invokes all registered handlers in a DI scope.
///
/// This is deliberately a single-process bus (publisher and consumers must run
/// in the same app). To go cross-service, replace this implementation with a
/// broker-backed one that serializes the event and publishes to a topic/queue;
/// the <see cref="IEventBus"/> and handler contracts stay the same.
/// </summary>
public sealed class InProcessEventBus : IEventBus
{
    private readonly Channel<IntegrationEvent> _channel;

    public InProcessEventBus(Channel<IntegrationEvent> channel) => _channel = channel;

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(@event);
        await _channel.Writer.WriteAsync(@event, cancellationToken);
    }
}

/// <summary>
/// Background service that dispatches queued integration events to their
/// registered <see cref="IIntegrationEventHandler{TEvent}"/> implementations.
/// </summary>
public sealed class EventBusDispatcher : BackgroundService
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();

    private readonly Channel<IntegrationEvent> _channel;
    private readonly IServiceProvider _services;
    private readonly ILogger<EventBusDispatcher> _logger;

    public EventBusDispatcher(
        Channel<IntegrationEvent> channel,
        IServiceProvider services,
        ILogger<EventBusDispatcher> logger)
    {
        _channel = channel;
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await DispatchAsync(@event, stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Failed to dispatch integration event {EventType} ({EventId})",
                    @event.EventType, @event.EventId);
            }
        }
    }

    private async Task DispatchAsync(IntegrationEvent @event, CancellationToken cancellationToken)
    {
        var handlerInterface = HandlerTypeCache.GetOrAdd(
            @event.GetType(),
            t => typeof(IIntegrationEventHandler<>).MakeGenericType(t));

        using var scope = _services.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerInterface).ToList();

        if (handlers.Count == 0)
        {
            _logger.LogDebug("No handlers registered for integration event {EventType}", @event.EventType);
            return;
        }

        var method = handlerInterface.GetMethod("HandleAsync")!;
        foreach (var handler in handlers)
        {
            if (handler is null) continue;
            var task = (Task)method.Invoke(handler, new object[] { @event, cancellationToken })!;
            await task;
        }
    }
}
