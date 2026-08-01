namespace Sales.Domain.Entities;

/// <summary>
/// Represents a sales order placed by a customer
/// </summary>
public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrderNumber { get; set; } = string.Empty;

    // Customer details
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    // Cross-module linkage (e.g., quote or CRM opportunity)
    public string? SourceModule { get; set; }
    public string? SourceReferenceId { get; set; }

    public Enums.OrderStatus Status { get; set; } = Enums.OrderStatus.Draft;
    public Enums.Currency Currency { get; set; } = Enums.Currency.USD;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public DateTime? CancelledDate { get; set; }

    // Shipping details
    public Enums.ShippingMethod? ShippingMethod { get; set; }
    public string? ShippingAddressLine1 { get; set; }
    public string? ShippingAddressLine2 { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingCountry { get; set; }
    public string? TrackingNumber { get; set; }

    // Billing details
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }

    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    public List<OrderLineItem> LineItems { get; set; } = new();
    public List<OrderPayment> Payments { get; set; } = new();

    // Computed financials
    public decimal Subtotal => LineItems.Sum(li => li.Subtotal);
    public decimal TotalDiscount => LineItems.Sum(li => li.DiscountAmount);
    public decimal TotalTax => LineItems.Sum(li => li.TaxAmount);
    public decimal ShippingCost { get; set; } = 0m;
    public decimal Total => LineItems.Sum(li => li.LineTotal) + ShippingCost;
    public decimal AmountPaid => Payments
        .Where(p => p.IsCompleted)
        .Sum(p => p.Amount);
    public decimal BalanceDue => Total - AmountPaid;

    public bool IsPaid => BalanceDue <= 0m && Total > 0m;

    // Tracking
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }

    // Metadata
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<string>? Tags { get; set; }
}

public class OrderLineItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrderId { get; set; } = string.Empty;

    public string? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Description { get; set; }

    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; } = 0;
    public decimal TaxPercentage { get; set; } = 0;

    public decimal Subtotal => Quantity * UnitPrice;
    public decimal DiscountAmount => Subtotal * (DiscountPercentage / 100);
    public decimal TaxableAmount => Subtotal - DiscountAmount;
    public decimal TaxAmount => TaxableAmount * (TaxPercentage / 100);
    public decimal LineTotal => TaxableAmount + TaxAmount;

    public int SortOrder { get; set; }

    // Fulfillment tracking
    public decimal QuantityShipped { get; set; } = 0;
    public decimal QuantityDelivered { get; set; } = 0;
    public bool IsFulfilled => QuantityDelivered >= Quantity;
}

public class OrderPayment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrderId { get; set; } = string.Empty;

    public Enums.PaymentMethod PaymentMethod { get; set; } = Enums.PaymentMethod.CreditCard;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }

    public bool IsCompleted { get; set; } = true;
}
