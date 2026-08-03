# Sales Event Integration

## Overview

The Sales module publishes integration events throughout the order and quote lifecycle, enabling other modules to react to sales activities in real-time. This document describes the Sales-specific events, their handlers, and integration patterns.

## Sales Integration Events

All Sales events are defined in `BusinessAsUsual.Core/Events/Integration/` and follow the standard `IntegrationEvent` pattern.

### 1. OrderCreatedIntegrationEvent

**Published by:** `Sales.Application.Services.OrderService.CreateOrderAsync()`  
**Event Type:** `"sales.order.created"`  
**When:** A new order is created (either from a quote or directly)

**Payload:**
```csharp
{
	OrderId: string,
	OrderNumber: string,
	CustomerId: string,
	CustomerName: string,
	TotalAmount: decimal,
	Currency: string,
	OrderDate: DateTime,
	LineItems: List<OrderLineItemDto>
}
```

**Current Consumers:** None (available for future integrations)

**Potential Use Cases:**
- CRM: Update customer engagement scores
- Finance: Prepare draft invoice
- Notifications: Email order confirmation to customer

---

### 2. OrderConfirmedIntegrationEvent

**Published by:** `Sales.Application.Services.OrderService.ConfirmOrderAsync()`  
**Event Type:** `"sales.order.confirmed"`  
**When:** An order moves to Confirmed status (payment received, ready for fulfillment)

**Payload:**
```csharp
{
	OrderId: string,
	OrderNumber: string,
	CustomerId: string,
	CustomerName: string,
	ConfirmedDate: DateTime,
	LineItems: List<OrderLineItemDto>
}
```

**Current Consumers:** None (available for Inventory stock reservation)

**Potential Use Cases:**
- Inventory: Reserve stock for confirmed orders
- Finance: Finalize invoice, record revenue
- Fulfillment: Create picking list

---

### 3. OrderShippedIntegrationEvent ⭐

**Published by:** `Sales.Application.Services.OrderService.ShipOrderAsync()`  
**Event Type:** `"sales.order.shipped"`  
**When:** An order is shipped to the customer

**Payload:**
```csharp
{
	OrderId: string,
	OrderNumber: string,
	CustomerId: string,
	ShippedDate: DateTime,
	TrackingNumber: string?,
	ShippingMethod: string,
	LineItems: List<OrderLineItemDto>
}
```

**Current Consumers:**
- ✅ **Inventory.API** → `OrderShippedEventHandler` decrements stock quantities

**Integration Flow:**
```
Sales.API (OrderService.ShipOrderAsync)
	↓ Publishes OrderShippedIntegrationEvent
	↓ Event Bus (InProcessEventBus)
	↓ Dispatches to handlers
	↓
Inventory.API (OrderShippedEventHandler)
	↓ Parses ProductId Guids
	↓ Finds StockItems by ProductId
	↓ Decrements QuantityOnHand
	↓ Records InventoryTransaction (Type: SalesOrder, Quantity: negative)
	↓ Logs success/warnings
```

**Stock Decrement Logic:**
1. Parse product IDs from string to Guid
2. Find all stock items for each product across warehouses
3. Decrement stock FIFO (highest quantity first)
4. Record an inventory transaction for audit trail
5. Log warnings if insufficient stock

**Potential Additional Use Cases:**
- CRM: Update customer order history
- Notifications: Email shipping confirmation with tracking link
- Finance: Trigger revenue recognition

---

### 4. QuoteConvertedIntegrationEvent

**Published by:** `Sales.Application.Services.QuoteService.ConvertQuoteToOrderAsync()`  
**Event Type:** `"sales.quote.converted"`  
**When:** A quote is converted into an order (quote status becomes Converted)

**Payload:**
```csharp
{
	QuoteId: string,
	QuoteNumber: string,
	OrderId: string,
	OrderNumber: string,
	CustomerId: string,
	CustomerName: string,
	TotalAmount: decimal,
	Currency: string,
	ConvertedDate: DateTime
}
```

**Current Consumers:** None (available for analytics/CRM)

**Potential Use Cases:**
- CRM: Track quote-to-order conversion rates, update opportunity stage
- Analytics: Sales pipeline velocity metrics
- Finance: Move from projected to confirmed revenue

---

## Event Registration Pattern

### Publisher Setup (Sales.API)

```csharp
// Program.cs
builder.Services.AddInProcessEventBus();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Services receive IEventBus via DI
public class OrderService : IOrderService
{
	private readonly IEventBus _eventBus;

	public OrderService(IEventBus eventBus) { _eventBus = eventBus; }

	public async Task<OrderDto> ShipOrderAsync(string id, string trackingNumber)
	{
		// ... update order status ...

		await _eventBus.PublishAsync(new OrderShippedIntegrationEvent
		{
			OrderId = order.Id,
			// ... populate event data ...
		});

		return MapToDto(order);
	}
}
```

### Consumer Setup (Inventory.API)

```csharp
// Program.cs
builder.Services.AddInProcessEventBus();
builder.Services.AddScoped<IIntegrationEventHandler<OrderShippedIntegrationEvent>, OrderShippedEventHandler>();

// EventHandlers/OrderShippedEventHandler.cs
public class OrderShippedEventHandler : IIntegrationEventHandler<OrderShippedIntegrationEvent>
{
	private readonly IStockItemRepository _stockRepository;
	private readonly ILogger<OrderShippedEventHandler> _logger;

	public async Task HandleAsync(OrderShippedIntegrationEvent @event, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Processing OrderShipped event for {OrderNumber}", @event.OrderNumber);
		// ... decrement stock ...
	}
}
```

---

## Cross-Module Integration Map

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Event Bus (In-Process)                       │
└─────────────────────────────────────────────────────────────────────┘
								   │
		┌──────────────────────────┼──────────────────────────┐
		│                          │                          │
   ┌────▼────┐              ┌──────▼──────┐          ┌───────▼──────┐
   │  Sales  │              │  Inventory  │          │     CRM      │
   │   API   │              │     API     │          │     API      │
   └────┬────┘              └──────┬──────┘          └───────┬──────┘
		│                          │                          │
		│ Publishes:               │ Consumes:                │ (Future)
		│ • OrderCreated           │ • OrderShipped           │ Consumes:
		│ • OrderConfirmed         │   → Decrement Stock      │ • OrderCreated
		│ • OrderShipped           │   → Record Transaction   │ • QuoteConverted
		│ • QuoteConverted         │                          │
		│                          │                          │
```

---

## Testing Event Flow

### Manual Test (Local Development)

1. **Start all services:**
   ```powershell
   # Terminal 1: Inventory API
   cd services/Inventory/Inventory.API
   dotnet run

   # Terminal 2: Sales API
   cd services/Sales/Sales.API
   dotnet run
   ```

2. **Create and ship an order:**
   ```powershell
   # Create order
   curl -X POST http://localhost:5143/api/sales/orders -H "Content-Type: application/json" -d @order.json

   # Ship order (triggers event)
   curl -X POST http://localhost:5143/api/sales/orders/{orderId}/ship -H "Content-Type: application/json" -d '{"trackingNumber":"1Z999AA10123456789"}'
   ```

3. **Verify in logs:**
   - Sales.API: `✅ Publishing OrderShippedIntegrationEvent...`
   - Inventory.API: `📦 Processing OrderShipped event for Order O-20250115-0001...`
   - Inventory.API: `✅ Decremented 10 of Product XYZ from warehouse...`

4. **Check database:**
   ```sql
   -- Inventory stock should be decremented
   SELECT ProductId, QuantityOnHand FROM StockItems WHERE ProductId = '...';

   -- Transaction should be recorded
   SELECT * FROM InventoryTransactions WHERE ReferenceType = 'SalesOrder' ORDER BY TransactionDate DESC;
   ```

### Integration Test Example

```csharp
[Fact]
public async Task OrderShipped_Should_DecrementInventory()
{
	// Arrange
	var services = new ServiceCollection();
	services.AddInProcessEventBus();
	services.AddScoped<IIntegrationEventHandler<OrderShippedIntegrationEvent>, OrderShippedEventHandler>();
	// ... register repositories, DbContext, etc.

	var provider = services.BuildServiceProvider();
	var eventBus = provider.GetRequiredService<IEventBus>();

	// Act
	await eventBus.PublishAsync(new OrderShippedIntegrationEvent
	{
		OrderId = "ORDER-123",
		OrderNumber = "O-2025-0001",
		LineItems = new List<OrderLineItemDto>
		{
			new() { ProductId = productGuid.ToString(), Quantity = 5 }
		}
	});

	await Task.Delay(100); // Wait for background dispatch

	// Assert
	var stock = await stockRepository.GetByProductIdAsync(productGuid);
	Assert.Equal(95, stock.QuantityOnHand); // Was 100, shipped 5
}
```

---

## Error Handling & Resilience

### Publisher Side (Sales)
- Events are published **after** database changes are persisted
- Publishing failures are logged but **do not roll back** the order
- Use fire-and-forget pattern (no retries in Sales)

### Consumer Side (Inventory)
- Handler exceptions are logged but **do not fail** the event bus
- Idempotency: handlers should check for duplicate processing (use `EventId`)
- Warnings logged for insufficient stock, but handler completes successfully

### Monitoring
```csharp
_logger.LogInformation("📦 Processing OrderShipped event for Order {OrderNumber}", @event.OrderNumber);
_logger.LogWarning("⚠️ Insufficient stock for product {ProductName}", lineItem.ProductName);
_logger.LogError(ex, "❌ Failed to decrement stock for product {ProductName}", lineItem.ProductName);
```

---

## Future Enhancements

### Planned Events
- `OrderCancelledIntegrationEvent` → Inventory releases reserved stock
- `OrderDeliveredIntegrationEvent` → CRM updates customer satisfaction tracking
- `QuoteSentIntegrationEvent` → CRM logs sales activity

### Potential Consumers
- **Finance Module:**
  - `OrderConfirmed` → Create invoice
  - `OrderShipped` → Recognize revenue

- **CRM Module:**
  - `OrderCreated` → Update customer lifetime value
  - `QuoteConverted` → Track sales rep performance

- **Notifications Service:**
  - `OrderShipped` → Email customer with tracking link
  - `OrderDelivered` → Request product review

### Broker Mode (Cross-Process)
Currently using `InProcessEventBus` (same-process dispatch). To enable cross-service communication:

1. Switch to `MassTransitEventBus` (see `EVENTBUS-DEPLOYMENT.md`)
2. Configure RabbitMQ connection in `appsettings.json`
3. No code changes needed in publishers/handlers!

---

## Related Documentation
- [EVENTBUS-DEPLOYMENT.md](EVENTBUS-DEPLOYMENT.md) - Event bus deployment and configuration
- [DEVELOPMENT-ROADMAP.md](DEVELOPMENT-ROADMAP.md) - Module integration roadmap
- [SALES-MODULE.md](SALES-MODULE.md) - Sales module documentation (if exists)

---

**Last Updated:** 2025-01-15  
**Status:** ✅ OrderShipped → Inventory integration LIVE  
**Next:** CRM consumers for customer engagement tracking
