namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a stock transfer between warehouses or bin locations
/// </summary>
public class StockTransfer
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid? FromBinLocationId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public Guid? ToBinLocationId { get; set; }
    public int Quantity { get; set; }
    public StockTransferStatus Status { get; set; } = StockTransferStatus.Draft;
    public DateTime RequestedDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string? ShippedBy { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse FromWarehouse { get; set; } = null!;
    public BinLocation? FromBinLocation { get; set; }
    public Warehouse ToWarehouse { get; set; } = null!;
    public BinLocation? ToBinLocation { get; set; }
}

public enum StockTransferStatus
{
    Draft,
    Requested,
    Approved,
    InTransit,
    Received,
    Cancelled
}
