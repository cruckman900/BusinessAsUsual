namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a cycle count (physical inventory count) for a product
/// </summary>
public class CycleCount
{
    public Guid Id { get; set; }
    public string CountNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public int SystemQuantity { get; set; }
    public int CountedQuantity { get; set; }
    public int Variance => CountedQuantity - SystemQuantity;
    public CycleCountStatus Status { get; set; } = CycleCountStatus.Pending;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CountedDate { get; set; }
    public string? CountedBy { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }
    public string? Notes { get; set; }
    public bool AdjustmentCreated { get; set; }
    public Guid? StockAdjustmentId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public BinLocation? BinLocation { get; set; }
}

public enum CycleCountStatus
{
    Pending,
    InProgress,
    Counted,
    Verified,
    Adjusted,
    Cancelled
}
