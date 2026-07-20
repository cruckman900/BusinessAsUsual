using Finance.Domain.Enums;

namespace Finance.Domain.Entities;

public class Invoice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string InvoiceNumber { get; set; } = string.Empty;

    // Party details
    public string? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }

    // Cross-module linkage (e.g., the CRM opportunity that generated this invoice)
    public string? SourceModule { get; set; }
    public string? SourceReferenceId { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public Currency Currency { get; set; } = Currency.USD;

    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? SentDate { get; set; }
    public DateTime? PaidDate { get; set; }

    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    public List<InvoiceLineItem> LineItems { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();

    // Computed financials
    public decimal Subtotal => LineItems.Sum(li => li.Subtotal);
    public decimal TotalDiscount => LineItems.Sum(li => li.DiscountAmount);
    public decimal TotalTax => LineItems.Sum(li => li.TaxAmount);
    public decimal Total => LineItems.Sum(li => li.LineTotal);
    public decimal AmountPaid => Payments
        .Where(p => p.Status == PaymentStatus.Completed)
        .Sum(p => p.Amount);
    public decimal BalanceDue => Total - AmountPaid;

    public bool IsPaid => BalanceDue <= 0m && Total > 0m;
    public bool IsOverdue => !IsPaid
        && DueDate.HasValue
        && DueDate.Value.Date < DateTime.UtcNow.Date
        && Status != InvoiceStatus.Cancelled;

    // Tracking
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }

    // Metadata
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<string>? Tags { get; set; }
}
