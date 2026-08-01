namespace Sales.Domain.Entities;

/// <summary>
/// Represents a sales quote/proposal sent to a customer
/// </summary>
public class Quote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string QuoteNumber { get; set; } = string.Empty;

    // Customer details
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    // Cross-module linkage (e.g., CRM opportunity that generated this quote)
    public string? SourceModule { get; set; }
    public string? SourceReferenceId { get; set; }

    public Enums.QuoteStatus Status { get; set; } = Enums.QuoteStatus.Draft;
    public Enums.Currency Currency { get; set; } = Enums.Currency.USD;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public DateTime? ConvertedDate { get; set; }
    public string? ConvertedToOrderId { get; set; }

    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    public List<QuoteLineItem> LineItems { get; set; } = new();

    // Computed financials
    public decimal Subtotal => LineItems.Sum(li => li.Subtotal);
    public decimal TotalDiscount => LineItems.Sum(li => li.DiscountAmount);
    public decimal TotalTax => LineItems.Sum(li => li.TaxAmount);
    public decimal Total => LineItems.Sum(li => li.LineTotal);

    public bool IsExpired => Status != Enums.QuoteStatus.Accepted 
        && Status != Enums.QuoteStatus.Converted
        && ExpiryDate.HasValue 
        && ExpiryDate.Value.Date < DateTime.UtcNow.Date;

    // Tracking
    public DateTime? LastModifiedDate { get; set; }

    // Metadata
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<string>? Tags { get; set; }
}

public class QuoteLineItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string QuoteId { get; set; } = string.Empty;

    public string? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
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
}
