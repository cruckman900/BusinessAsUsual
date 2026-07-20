namespace Finance.Domain.Entities;

public class InvoiceLineItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string InvoiceId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1m;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }

    // Computed
    public decimal Subtotal => Quantity * UnitPrice;
    public decimal DiscountAmount => Subtotal * (DiscountPercent / 100m);
    public decimal TaxableAmount => Subtotal - DiscountAmount;
    public decimal TaxAmount => TaxableAmount * (TaxPercent / 100m);
    public decimal LineTotal => TaxableAmount + TaxAmount;

    // Optional cross-module linkage (e.g., CRM product category)
    public string? ProductCategory { get; set; }
}
