using Finance.Domain.Enums;

namespace Finance.Application.DTOs;

public class BillLineItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1m;
    public decimal UnitPrice { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal LineTotal { get; set; }
    public string? ExpenseCategory { get; set; }
}

public class BillDto
{
    public string Id { get; set; } = string.Empty;
    public string BillNumber { get; set; } = string.Empty;
    public string? VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? VendorEmail { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    public List<BillLineItemDto> LineItems { get; set; } = new();

    // Computed financials
    public decimal Subtotal { get; set; }
    public decimal TotalTax { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public bool IsPaid { get; set; }
    public bool IsOverdue { get; set; }

    public DateTime CreatedDate { get; set; }
    public List<string>? Tags { get; set; }
}

public class CreateBillLineItemRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1m;
    public decimal UnitPrice { get; set; }
    public decimal TaxPercent { get; set; }
    public string? ExpenseCategory { get; set; }
}

public class CreateBillRequest
{
    public string? VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? VendorEmail { get; set; }
    public Currency Currency { get; set; } = Currency.USD;
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public List<CreateBillLineItemRequest> LineItems { get; set; } = new();
    public List<string>? Tags { get; set; }
}

public class UpdateBillRequest
{
    public string? VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? VendorEmail { get; set; }
    public BillStatus Status { get; set; }
    public Currency Currency { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public List<CreateBillLineItemRequest> LineItems { get; set; } = new();
    public List<string>? Tags { get; set; }
}
