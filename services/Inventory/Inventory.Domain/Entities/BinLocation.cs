namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a specific bin or location within a warehouse
/// </summary>
public class BinLocation
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty; // e.g., A1-01-01
    public string? Aisle { get; set; }
    public string? Row { get; set; }
    public string? Shelf { get; set; }
    public string? Section { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
