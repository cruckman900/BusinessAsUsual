using Finance.Domain.Enums;

namespace Finance.Application.DTOs;

public class VendorPaymentDto
{
    public string Id { get; set; } = string.Empty;
    public string? BillId { get; set; }
    public string? BillNumber { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class RecordVendorPaymentRequest
{
    public string? BillId { get; set; }
    public string? VendorId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; } = Currency.USD;
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public DateTime? PaymentDate { get; set; }
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
}
