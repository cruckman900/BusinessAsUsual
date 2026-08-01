using Sales.Domain.Enums;

namespace Sales.Application.DTOs;

public class QuoteDto
{
    public string Id { get; set; } = string.Empty;
    public string QuoteNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public QuoteStatus Status { get; set; }
    public Currency Currency { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? SentDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public DateTime? ConvertedDate { get; set; }
    public string? ConvertedToOrderId { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public List<QuoteLineItemDto> LineItems { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal Total { get; set; }
    public bool IsExpired { get; set; }
}

public class QuoteLineItemDto
{
    public string Id { get; set; } = string.Empty;
    public string QuoteId { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public int SortOrder { get; set; }
}

public class CreateQuoteDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public Currency Currency { get; set; } = Currency.USD;
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public string? AssignedToEmployeeId { get; set; }
    public List<CreateQuoteLineItemDto> LineItems { get; set; } = new();
}

public class CreateQuoteLineItemDto
{
    public string? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; } = 0;
    public decimal TaxPercentage { get; set; } = 0;
    public int SortOrder { get; set; }
}

public class UpdateQuoteDto : CreateQuoteDto
{
    public string Id { get; set; } = string.Empty;
    public QuoteStatus Status { get; set; }
}
