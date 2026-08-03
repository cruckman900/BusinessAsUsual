# Cross-Module Event Integration & UI Enhancements

## Overview
This document describes the event-driven integration between Sales, Finance, CRM, and Inventory modules, plus UI improvements for Sales Web.

## Phase 1: Event Consumers in Finance & CRM

### Finance Module Integration
**Handler**: `Finance.Application.Events.OrderConfirmedEventHandler`
**Trigger**: `sales.order.confirmed` event
**Action**: Automatically creates a draft invoice when a Sales order is confirmed

#### Key Features
- Links invoice to source order via `SourceModule = "sales"` and `SourceReferenceId = orderId`
- Converts order line items to invoice line items
- Applies Net 30 payment terms by default
- Logs creation success/failure with emoji indicators (💰, ✅, ❌)
- Graceful error handling - failures don't break order confirmation

#### Registration
```csharp
// Finance.API/Program.cs
builder.Services.AddEventBus(builder.Configuration, bus =>
{
	bus.AddHandler<OrderConfirmedIntegrationEvent, OrderConfirmedEventHandler>();
});
```

### CRM Module Integration
**Handler**: `CRM.Application.Events.OrderCreatedEventHandler`
**Trigger**: `sales.order.created` event
**Action**: Logs order as a customer activity for sales rep visibility

#### Key Features
- Creates activity of type `Note` in CRM
- Links activity to customer via `CustomerId`
- Includes order summary (item count, total amount)
- Provides full purchase history visibility for sales reps
- Graceful error handling - failures don't break order creation

#### Registration
```csharp
// CRM.API/Program.cs
builder.Services.AddEventBus(builder.Configuration, bus =>
{
	bus.AddHandler<OrderCreatedIntegrationEvent, OrderCreatedEventHandler>();
});
```

## Phase 2: Sales Web UI Pickers ✅ COMPLETE

### ProductPicker Component
**Location**: `Sales.Web/Components/Shared/ProductPicker.razor`

Reusable Blazor component for selecting products from Inventory.

#### Features
- MudBlazor `MudAutocomplete` with search
- Fetches products from Inventory API via `InventoryApi` HttpClient
- Displays product name, SKU, and price in dropdown
- Searchable by name or SKU
- Lazy-loaded (fetches on first use)
- Configurable: Label, Variant, Margin, Dense, Required, Disabled

#### Usage Example
```razor
<ProductPicker @bind-SelectedProduct="selectedProduct"
			   Label="Choose Product"
			   Required="true" />

@code {
	ProductPicker.ProductDto? selectedProduct;
}
```

### CustomerPicker Component
**Location**: `Sales.Web/Components/Shared/CustomerPicker.razor`

Reusable Blazor component for selecting customers from CRM.

#### Features
- MudBlazor `MudAutocomplete` with search
- Fetches customers from CRM API via `CrmApi` HttpClient
- Displays customer name, email, and phone in dropdown
- Searchable by name, email, or phone
- Lazy-loaded (fetches on first use)
- Configurable: Label, Variant, Margin, Dense, Required, Disabled

#### Usage Example
```razor
<CustomerPicker @bind-SelectedCustomer="selectedCustomer"
				Label="Select Customer"
				Required="true" />

@code {
	CustomerPicker.CustomerDto? selectedCustomer;
}
```

### Integrated User Experience ✅ IMPROVED

Both `ProductPicker` and `CustomerPicker` are now **fully integrated** into `QuoteForm.razor` and `OrderForm.razor` using a clean inline expansion panel design (no nested dialogs):

#### Customer Selection Flow
1. User opens Create Quote or Create Order dialog
2. **Customer Info tab** displays:
   - CustomerPicker autocomplete at the top (searchable by name, email, phone, company, or industry)
   - **As you type** → Dropdown shows matching customers (case-insensitive LIKE search)
   - Type "enterprise" → Shows all customers with "Enterprise" anywhere in name/company/industry
   - When customer selected → Info alert shows customer details
   - Customer Name, Email, Phone, and ID fields auto-fill and become read-only
   - User can still manually enter custom customer if not using CRM
3. Selection triggers `OnParametersSet()` lifecycle hook for auto-fill

#### Product Selection Flow (✨ Redesigned - No More Nested Dialogs!)
1. User navigates to **Line Items tab**
2. **Collapsible "Add Product from Inventory" panel** (click to expand)
   - No more confusing nested dialog with two cancel buttons!
   - Inline expansion panel keeps everything in one dialog
3. Inside the panel:
   - ProductPicker autocomplete (searchable by name, SKU, description, or category)
   - **As you type** → Dropdown shows matching products (case-insensitive LIKE search)
   - Type "laptop" → Shows all products with "laptop" in name/SKU/description/category
   - Exact matches prioritized first, then alphabetical
   - Product details card: name, SKU, description, price
   - Quantity input (min 0.01, defaults to 1)
   - Live total: `quantity × price`
4. Click **"Add to Quote/Order"**:
   - Line item added with auto-filled ProductId, Name, SKU, Description, Unit Price
   - Panel collapses automatically
   - User can adjust discount/tax inline in the table
5. **"Add Custom Item"** button available for manual entry (fallback option)

#### Search Improvements ✅
**CustomerPicker** now searches across:
- Name (prioritizes starts-with matches)
- Email
- Phone
- Company
- Industry
- **All case-insensitive**, returns top 20 matches

**ProductPicker** now searches across:
- Name (prioritizes starts-with matches)
- SKU
- Description
- Category
- **All case-insensitive**, returns top 20 matches

**Example:** Type "ent" and you'll see:
- "Enterprise CRM Platform" (name starts with "Ent")
- "Client Management Enterprise Edition" (contains "Enterprise")
- Products with "entertainment" in description
- All sorted with best matches first

#### Benefits of Integration
- ✅ **Data consistency** - Product prices and customer info sync from source systems
- ✅ **Reduced errors** - No typos in customer names or manual price entry mistakes
- ✅ **Faster workflow** - Search and select vs. typing all details
- ✅ **Dual mode** - Use CRM/Inventory data OR enter custom values as fallback
- ✅ **Traceability** - ProductId and CustomerId link back to source records for reporting

## Event Flow Diagrams

### Order Creation Flow
```
1. Sales.Web (User creates order)
   ↓
2. Sales.API (OrderService.CreateOrderAsync)
   ↓ Publishes
3. OrderCreatedIntegrationEvent ("sales.order.created")
   ↓ Consumed by
4. CRM.OrderCreatedEventHandler
   ↓
5. CRM Activity logged for customer
```

### Order Confirmation Flow
```
1. Sales.Web (User confirms order/payment received)
   ↓
2. Sales.API (OrderService.ConfirmOrderAsync)
   ↓ Publishes
3. OrderConfirmedIntegrationEvent ("sales.order.confirmed")
   ↓ Consumed by
4. Finance.OrderConfirmedEventHandler
   ↓
5. Draft invoice created in Finance
```

### Order Shipping Flow
```
1. Sales.Web (User ships order)
   ↓
2. Sales.API (OrderService.ShipOrderAsync)
   ↓ Publishes
3. OrderShippedIntegrationEvent ("sales.order.shipped")
   ↓ Consumed by
4. Inventory.OrderShippedEventHandler
   ↓
5. Stock decremented + transaction logged
```

## Testing the Integration

### 1. Create an Order in Sales
```bash
POST http://localhost:5001/api/orders
{
  "customerId": "guid-from-crm",
  "customerName": "Acme Corp",
  "lineItems": [...]
}
```

**Expected Result**: CRM activity created for the customer

### 2. Confirm the Order
```bash
PUT http://localhost:5001/api/orders/{id}/confirm
```

**Expected Result**: Draft invoice created in Finance

### 3. Ship the Order
```bash
PUT http://localhost:5001/api/orders/{id}/ship
```

**Expected Result**: Inventory stock decremented

### Verify Event Handlers
Check logs for emoji indicators:
- 💰 Finance processing order confirmation
- ✅ Success messages with invoice/activity IDs
- ❌ Error messages if handlers fail
- 📋 CRM processing order creation
- 📦 Inventory processing shipment (from prior integration)

## Architecture Notes

### In-Process Event Bus
Currently uses `InProcessEventBus` from `BusinessAsUsual.Core`.
- Events are delivered within the same process
- All modules must be running in the same application (e.g., `BusinessAsUsual.Web`)
- For true microservices, replace with RabbitMQ/Azure Service Bus

### Error Handling Strategy
All event handlers use try-catch with logging and **do not throw**.
- Rationale: A failure in one consumer shouldn't break the source operation
- Example: If Finance can't create an invoice, the Sales order still confirms successfully
- Manual fallback: Operators can create invoices/activities manually if auto-creation fails

### Event Contracts
All integration events are defined in `BusinessAsUsual.Core/Events/Integration/`:
- `OrderCreatedIntegrationEvent`
- `OrderConfirmedIntegrationEvent`
- `OrderShippedIntegrationEvent`
- `QuoteConvertedIntegrationEvent`

Shared `OrderLineItemDto` ensures consistency across modules.

## Future Enhancements (Pending)

### New Events (Next Phase)
- `OrderCancelledIntegrationEvent` - notify Inventory/Finance when orders are cancelled
- `OrderDeliveredIntegrationEvent` - trigger final accounting/close-out workflows

### UI Enhancements (Next Phase)
- ✅ **Integrated `ProductPicker` into `QuoteForm.razor` and `OrderForm.razor`** - Users can now browse and select products from Inventory
- ✅ **Integrated `CustomerPicker` into `QuoteForm.razor` and `OrderForm.razor`** - Users can now select customers from CRM with auto-fill
- ✅ **Product price auto-population** - When product is selected, price is automatically filled
- ✅ **Customer contact info auto-population** - When customer is selected, all contact details are auto-filled
- Dual mode support: Select from CRM/Inventory OR enter custom values manually

## Related Documentation
- [Sales Event Integration](SALES-EVENT-INTEGRATION.md) - Original Sales/Inventory integration
- [Development Roadmap](DEVELOPMENT-ROADMAP.md) - Overall project status
- `BusinessAsUsual.Core/Events/README.md` - Event bus architecture (if exists)

## Change History
- **2025-01-XX**: Phase 1 (Finance/CRM consumers) and Phase 2 (UI pickers) implemented
- **Previous**: Sales/Inventory integration completed
