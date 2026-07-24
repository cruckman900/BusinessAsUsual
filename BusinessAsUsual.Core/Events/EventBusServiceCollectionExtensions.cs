using System.Threading.Channels;
using MassTransit;
using Microsoft.Extensions.Configuration;
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
    /// <remarks>
    /// Prefer <see cref="AddEventBus"/> for new code: it adds a config toggle so
    /// the same registration can run in-process (dev) or over a real broker
    /// (RabbitMQ / Amazon MQ) without changing publishers or handlers.
    /// </remarks>
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

    /// <summary>
    /// Registers the event bus using the provider selected by configuration
    /// (<c>EventBus:Provider</c> = <c>InProcess</c> | <c>Broker</c>, default
    /// <c>InProcess</c>). Handlers are declared via the <see cref="EventBusBuilder"/>
    /// so the same declaration wires either the in-process dispatcher or the
    /// MassTransit/RabbitMQ transport (with exponential-backoff retry and a
    /// dead-letter <c>*_error</c> queue) without touching publisher/handler code.
    /// </summary>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<EventBusBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new EventBusBuilder(services);
        configure(builder);

        var provider = configuration["EventBus:Provider"];
        var useBroker = string.Equals(provider, "Broker", StringComparison.OrdinalIgnoreCase);

        if (useBroker)
        {
            AddBrokerBus(services, configuration, builder);
        }
        else
        {
            services.AddInProcessEventBus();
        }

        return services;
    }

    private static void AddBrokerBus(
        IServiceCollection services,
        IConfiguration configuration,
        EventBusBuilder builder)
    {
        var rabbit = configuration.GetSection("EventBus:RabbitMq");
        var host = rabbit["Host"] ?? "localhost";
        var virtualHost = rabbit["VirtualHost"] ?? "/";
        var username = rabbit["Username"] ?? "guest";
        var password = rabbit["Password"] ?? "guest";
        var retryLimit = rabbit.GetValue("RetryLimit", 5);

        services.AddMassTransit(x =>
        {
            foreach (var registerConsumer in builder.ConsumerRegistrations)
            {
                registerConsumer(x);
            }

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, virtualHost, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                // Exponential-backoff retry. When the limit is exhausted MassTransit
                // moves the message to the transport dead-letter queue (<queue>_error).
                cfg.UseMessageRetry(r => r.Exponential(
                    retryLimit,
                    minInterval: TimeSpan.FromSeconds(1),
                    maxInterval: TimeSpan.FromMinutes(5),
                    intervalDelta: TimeSpan.FromSeconds(2)));

                cfg.ConfigureEndpoints(context);
            });
        });

        services.TryAddSingleton<IEventBus, MassTransitEventBus>();
    }
}

/// <summary>
/// Declares the integration-event handlers for <see cref="EventBusServiceCollectionExtensions.AddEventBus"/>.
/// Each <see cref="AddHandler{TEvent, THandler}"/> call registers the handler in DI and,
/// for broker mode, records the generic MassTransit consumer that adapts it.
/// </summary>
public sealed class EventBusBuilder
{
    private readonly IServiceCollection _services;
    private readonly HashSet<Type> _consumerEventTypes = new();

    internal List<Action<IBusRegistrationConfigurator>> ConsumerRegistrations { get; } = new();

    internal EventBusBuilder(IServiceCollection services) => _services = services;

    public EventBusBuilder AddHandler<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        _services.AddScoped<IIntegrationEventHandler<TEvent>, THandler>();

        // Register the bridging consumer once per event type, even when several
        // handlers target the same event (the consumer fans out to all of them).
        if (_consumerEventTypes.Add(typeof(TEvent)))
        {
            ConsumerRegistrations.Add(x => x.AddConsumer<IntegrationEventConsumer<TEvent>>());
        }

        return this;
    }
}
