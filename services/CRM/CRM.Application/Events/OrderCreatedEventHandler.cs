using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Events;

/// <summary>
/// When Sales creates an order, log it as a customer activity in CRM.
/// This provides full visibility into the customer's purchase history and
/// helps sales reps understand customer engagement.
/// </summary>
public sealed class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IActivityService _activityService;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(IActivityService activityService, ILogger<OrderCreatedEventHandler> logger)
    {
        _activityService = activityService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "📋 Processing OrderCreated event for Order {OrderNumber} - logging CRM activity",
            @event.OrderNumber);

        try
        {
            // Calculate total for the activity description
            decimal total = @event.LineItems.Sum(li => li.Quantity * li.UnitPrice);
            int itemCount = @event.LineItems.Count;

            var request = new CreateActivityRequest
            {
                Type = ActivityType.Note, // Use Note type to log the order event
                Subject = $"Order Placed: {@event.OrderNumber}",
                Description = $"Customer placed order {@event.OrderNumber} with {itemCount} item(s) totaling ${total:N2} {@event.Currency}. " +
                              $"Order Date: {@event.OrderDate:yyyy-MM-dd}",
                ActivityDate = @event.OrderDate,
                Priority = Priority.Medium,
                CustomerId = @event.CustomerId
            };

            var activity = await _activityService.CreateActivityAsync(request);

            _logger.LogInformation(
                "✅ Logged CRM activity {ActivityId} for order {OrderNumber} (Customer: {CustomerName})",
                activity.Id, @event.OrderNumber, @event.CustomerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Failed to create CRM activity for order {OrderNumber}",
                @event.OrderNumber);

            // Don't throw - event handler failures shouldn't break the order creation
            // The activity can be logged manually if auto-creation fails
        }
    }
}
