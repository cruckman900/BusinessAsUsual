using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Inventory.API.EventHandlers;

/// <summary>
/// Handles OrderShippedIntegrationEvent from Sales module by decrementing
/// inventory stock quantities for shipped products.
/// </summary>
public class OrderShippedEventHandler : IIntegrationEventHandler<OrderShippedIntegrationEvent>
{
    private readonly IStockItemRepository _stockRepository;
    private readonly IInventoryTransactionRepository _transactionRepository;
    private readonly ILogger<OrderShippedEventHandler> _logger;

    public OrderShippedEventHandler(
        IStockItemRepository stockRepository,
        IInventoryTransactionRepository transactionRepository,
        ILogger<OrderShippedEventHandler> logger)
    {
        _stockRepository = stockRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task HandleAsync(OrderShippedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "📦 Processing OrderShipped event for Order {OrderNumber} with {LineItemCount} items",
            @event.OrderNumber, @event.LineItems.Count);

        foreach (var lineItem in @event.LineItems)
        {
            try
            {
                // Parse ProductId as Guid (Sales sends string IDs, Inventory uses Guid)
                if (!Guid.TryParse(lineItem.ProductId, out var productGuid))
                {
                    _logger.LogWarning(
                        "⚠️  Invalid ProductId format for {ProductName}: {ProductId}",
                        lineItem.ProductName, lineItem.ProductId);
                    continue;
                }

                // Find stock items for this product
                var allStockItems = await _stockRepository.GetAllAsync();
                var stockItems = allStockItems
                    .Where(si => si.ProductId == productGuid)
                    .ToList();

                if (!stockItems.Any())
                {
                    _logger.LogWarning(
                        "⚠️  No stock items found for product {ProductName} (ID: {ProductId})",
                        lineItem.ProductName, lineItem.ProductId);
                    continue;
                }

                int remainingQuantity = (int)lineItem.Quantity;

                // Decrement stock from available warehouses (FIFO or prioritized by quantity available)
                foreach (var stockItem in stockItems.OrderByDescending(s => s.QuantityOnHand))
                {
                    if (remainingQuantity <= 0) break;

                    var quantityToDecrement = Math.Min(stockItem.QuantityOnHand, remainingQuantity);
                    if (quantityToDecrement <= 0) continue;

                    stockItem.QuantityOnHand -= quantityToDecrement;
                    stockItem.UpdatedAt = DateTime.UtcNow;
                    await _stockRepository.UpdateAsync(stockItem);

                    // Record inventory transaction
                    var transaction = new InventoryTransaction
                    {
                        ProductId = stockItem.ProductId,
                        WarehouseId = stockItem.WarehouseId,
                        Type = TransactionType.SalesOrder,
                        Quantity = -quantityToDecrement, // Negative for outbound
                        UnitCost = 0, // Cost tracking can be improved later
                        ReferenceType = "SalesOrder",
                        ReferenceId = Guid.TryParse(@event.OrderId, out var orderGuid) ? orderGuid : null,
                        TransactionDate = @event.ShippedDate,
                        Notes = $"Stock decremented for shipped order {@event.OrderNumber}",
                        CreatedBy = "SalesEventHandler"
                    };
                    await _transactionRepository.AddAsync(transaction);

                    remainingQuantity -= quantityToDecrement;

                    _logger.LogInformation(
                        "✅ Decremented {Quantity} of {ProductName} from warehouse (remaining to ship: {Remaining})",
                        quantityToDecrement, lineItem.ProductName, remainingQuantity);
                }

                if (remainingQuantity > 0)
                {
                    _logger.LogWarning(
                        "⚠️  Insufficient stock for product {ProductName}: needed {Needed} but only {Shipped} available",
                        lineItem.ProductName, (int)lineItem.Quantity, (int)lineItem.Quantity - remainingQuantity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Failed to decrement stock for product {ProductName} in order {OrderNumber}",
                    lineItem.ProductName, @event.OrderNumber);
            }
        }

        _logger.LogInformation("✅ Completed OrderShipped event processing for {OrderNumber}", @event.OrderNumber);
    }
}
