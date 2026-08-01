namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a product in the inventory system
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    public int ReorderPoint { get; set; }
    public int ReorderQuantity { get; set; }
    public string? Category { get; set; }
    public string UnitOfMeasure { get; set; } = "EA"; // EA, BOX, CASE, etc.
    public bool IsActive { get; set; } = true;
    public bool IsTrackedInventory { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
}
