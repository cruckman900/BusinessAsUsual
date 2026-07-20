using Finance.Domain.Enums;

namespace Finance.Domain.Entities;

public class Payment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? InvoiceId { get; set; }
    public string? CustomerId { get; set; }

    public decimal Amount { get; set; }
    public Currency Currency { get; set; } = Currency.USD;
    public PaymentMethod Method { get; set; } = PaymentMethod.Unknown;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }

    // Tracking
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedDate { get; set; }
}
