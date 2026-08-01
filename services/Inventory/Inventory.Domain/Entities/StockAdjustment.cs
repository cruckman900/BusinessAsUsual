namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a manual stock adjustment (increase or decrease)
/// </summary>
public class StockAdjustment
{
    public Guid Id { get; set; }
    public string AdjustmentNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public int QuantityChange { get; set; } // Positive = increase, Negative = decrease
    public StockAdjustmentType Type { get; set; }
    public StockAdjustmentReason Reason { get; set; }
    public string? ReasonNotes { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime AdjustmentDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public BinLocation? BinLocation { get; set; }
}

public enum StockAdjustmentType
{
    Increase,
    Decrease
}

public enum StockAdjustmentReason
{
    Damage,
    Loss,
    Theft,
    Found,
    CycleCount,
    Donation,
    Return,
    Correction,
    Other
}
