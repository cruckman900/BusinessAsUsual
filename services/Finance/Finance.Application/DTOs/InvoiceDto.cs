using Finance.Domain.Enums;

namespace Finance.Application.DTOs;

public class InvoiceLineItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1m;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal LineTotal { get; set; }
    public string? ProductCategory { get; set; }
}

public class InvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceReferenceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? SentDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }

    public List<InvoiceLineItemDto> LineItems { get; set; } = new();

    // Computed financials
    public decimal Subtotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public bool IsPaid { get; set; }
    public bool IsOverdue { get; set; }

    public DateTime CreatedDate { get; set; }
    public List<string>? Tags { get; set; }
}

public class CreateInvoiceLineItemRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1m;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public string? ProductCategory { get; set; }
}

public class CreateInvoiceRequest
{
    public string? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public Currency Currency { get; set; } = Currency.USD;
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public string? SourceModule { get; set; }
    public string? SourceReferenceId { get; set; }
    public List<CreateInvoiceLineItemRequest> LineItems { get; set; } = new();
    public List<string>? Tags { get; set; }
}

public class UpdateInvoiceRequest
{
    public string? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public InvoiceStatus Status { get; set; }
    public Currency Currency { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public List<CreateInvoiceLineItemRequest> LineItems { get; set; } = new();
    public List<string>? Tags { get; set; }
}
