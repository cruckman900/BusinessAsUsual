namespace Inventory.Application.DTOs;

public class StockItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid? BinLocationId { get; set; }
    public string? BinLocationCode { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityAllocated { get; set; }
    public int QuantityAvailable { get; set; }
    public decimal AverageCost { get; set; }
    public DateTime LastStockDate { get; set; }
}

public class StockSummaryDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public int TotalOnHand { get; set; }
    public int TotalAllocated { get; set; }
    public int TotalAvailable { get; set; }
    public decimal TotalValue { get; set; }
    public List<StockItemDto> LocationBreakdown { get; set; } = new();
}
