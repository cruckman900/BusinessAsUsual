using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BusinessAsUsual.Core.Events;

public static class EventBusServiceCollectionExtensions
{
    /// <summary>
    /// Registers the in-process event bus: a shared unbounded channel, the
    /// <see cref="IEventBus"/> publisher, and the background dispatcher.
    /// Register handlers separately with <see cref="AddIntegrationEventHandler{TEvent, THandler}"/>.
    /// </summary>
    public static IServiceCollection AddInProcessEventBus(this IServiceCollection services)
    {
        services.TryAddSingleton(_ => Channel.CreateUnbounded<IntegrationEvent>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            }));

        services.TryAddSingleton<IEventBus, InProcessEventBus>();
        services.AddHostedService<EventBusDispatcher>();
        return services;
    }

    /// <summary>
    /// Registers a handler for a given integration event type. Multiple
    /// handlers may be registered for the same event.
    /// </summary>
    public static IServiceCollection AddIntegrationEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        services.AddScoped<IIntegrationEventHandler<TEvent>, THandler>();
        return services;
    }
}
