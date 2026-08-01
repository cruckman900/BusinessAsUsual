namespace Inventory.Domain.Entities;

/// <summary>
/// Represents the actual stock of a product in a specific location
/// </summary>
public class StockItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityAllocated { get; set; } // Reserved for orders
    public int QuantityAvailable => QuantityOnHand - QuantityAllocated;
    public decimal AverageCost { get; set; }
    public DateTime LastStockDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public BinLocation? BinLocation { get; set; }
}
