namespace Inventory.Application.DTOs;

public class InventoryTransactionDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public int RunningBalance { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class CreateStockAdjustmentDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}

public class CreateStockTransferDto
{
    public Guid ProductId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid? FromBinLocationId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public Guid? ToBinLocationId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}
