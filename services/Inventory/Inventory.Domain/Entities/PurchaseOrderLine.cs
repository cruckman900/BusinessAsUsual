namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a line item in a purchase order
/// </summary>
public class PurchaseOrderLine
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public int QuantityReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal => Quantity * UnitPrice * (1 + TaxRate);
    public string? Notes { get; set; }

    // Navigation properties
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
