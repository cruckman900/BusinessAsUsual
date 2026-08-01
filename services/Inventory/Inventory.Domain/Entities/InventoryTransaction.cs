namespace Inventory.Domain.Entities;

/// <summary>
/// Represents any inventory movement transaction
/// </summary>
public class InventoryTransaction
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public TransactionType Type { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost => Quantity * UnitCost;
    public int RunningBalance { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReferenceType { get; set; } // PO, Adjustment, Transfer, etc.
    public Guid? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public BinLocation? BinLocation { get; set; }
}

public enum TransactionType
{
    PurchaseOrder,
    StockAdjustment,
    Transfer,
    SalesOrder,
    ReturnToSupplier,
    CustomerReturn,
    CycleCount,
    Manufacturing
}
