using Finance.Application.DTOs;
using Finance.Domain.Entities;
using Finance.Domain.Enums;

namespace Finance.Application.Services;

public static class FinanceMappers
{
    public static InvoiceDto ToDto(this Invoice inv) => new()
    {
        Id = inv.Id,
        InvoiceNumber = inv.InvoiceNumber,
        CustomerId = inv.CustomerId,
        CustomerName = inv.CustomerName,
        CustomerEmail = inv.CustomerEmail,
        SourceModule = inv.SourceModule,
        SourceReferenceId = inv.SourceReferenceId,
        Status = inv.Status.ToString(),
        Currency = inv.Currency.ToString(),
        IssueDate = inv.IssueDate,
        DueDate = inv.DueDate,
        SentDate = inv.SentDate,
        PaidDate = inv.PaidDate,
        Notes = inv.Notes,
        Terms = inv.Terms,
        AssignedToEmployeeId = inv.AssignedToEmployeeId,
        LineItems = inv.LineItems.Select(li => li.ToDto()).ToList(),
        Subtotal = inv.Subtotal,
        TotalDiscount = inv.TotalDiscount,
        TotalTax = inv.TotalTax,
        Total = inv.Total,
        AmountPaid = inv.AmountPaid,
        BalanceDue = inv.BalanceDue,
        IsPaid = inv.IsPaid,
        IsOverdue = inv.IsOverdue,
        CreatedDate = inv.CreatedDate,
        Tags = inv.Tags
    };

    public static InvoiceLineItemDto ToDto(this InvoiceLineItem li) => new()
    {
        Id = li.Id,
        Description = li.Description,
        Quantity = li.Quantity,
        UnitPrice = li.UnitPrice,
        DiscountPercent = li.DiscountPercent,
        TaxPercent = li.TaxPercent,
        LineTotal = li.LineTotal,
        ProductCategory = li.ProductCategory
    };

    public static PaymentDto ToDto(this Payment p, string? invoiceNumber = null) => new()
    {
        Id = p.Id,
        InvoiceId = p.InvoiceId,
        InvoiceNumber = invoiceNumber,
        CustomerId = p.CustomerId,
        Amount = p.Amount,
        Currency = p.Currency.ToString(),
        Method = p.Method.ToString(),
        Status = p.Status.ToString(),
        PaymentDate = p.PaymentDate,
        TransactionReference = p.TransactionReference,
        Notes = p.Notes,
        CreatedDate = p.CreatedDate
    };

    public static InvoiceLineItem ToEntity(this CreateInvoiceLineItemRequest r, string invoiceId) => new()
    {
        InvoiceId = invoiceId,
        Description = r.Description,
        Quantity = r.Quantity,
        UnitPrice = r.UnitPrice,
        DiscountPercent = r.DiscountPercent,
        TaxPercent = r.TaxPercent,
        ProductCategory = r.ProductCategory
    };
}
