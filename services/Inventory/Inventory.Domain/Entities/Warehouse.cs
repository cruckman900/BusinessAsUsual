namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a physical warehouse or storage location
/// </summary>
public class Warehouse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public string? ManagerName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<BinLocation> BinLocations { get; set; } = new List<BinLocation>();
    public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
