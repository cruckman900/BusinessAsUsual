using Inventory.Application.DTOs;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class PurchaseOrderService
{
    private readonly IPurchaseOrderRepository _poRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IInventoryTransactionRepository _transactionRepository;

    public PurchaseOrderService(
        IPurchaseOrderRepository poRepository,
        ISupplierRepository supplierRepository,
        IWarehouseRepository warehouseRepository,
        IProductRepository productRepository,
        IStockItemRepository stockItemRepository,
        IInventoryTransactionRepository transactionRepository)
    {
        _poRepository = poRepository;
        _supplierRepository = supplierRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _stockItemRepository = stockItemRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync()
    {
        var orders = await _poRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(Guid id)
    {
        var order = await _poRepository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<IEnumerable<PurchaseOrderDto>> GetPurchaseOrdersBySupplierAsync(Guid supplierId)
    {
        var orders = await _poRepository.GetBySupplierIdAsync(supplierId);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<PurchaseOrderDto>> GetPurchaseOrdersByStatusAsync(string status)
    {
        var statusEnum = Enum.Parse<PurchaseOrderStatus>(status, true);
        var orders = await _poRepository.GetByStatusAsync(statusEnum);
        return orders.Select(MapToDto);
    }

    public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
        if (supplier == null) throw new Exception("Supplier not found");

        var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
        if (warehouse == null) throw new Exception("Warehouse not found");

        var order = new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            OrderNumber = await GenerateOrderNumber(),
            SupplierId = dto.SupplierId,
            WarehouseId = dto.WarehouseId,
            OrderDate = dto.OrderDate,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            ShippingCost = dto.ShippingCost,
            Notes = dto.Notes,
            Status = PurchaseOrderStatus.Draft,
            CreatedBy = "System",
            CreatedAt = DateTime.UtcNow
        };

        var lines = new List<PurchaseOrderLine>();
        foreach (var lineDto in dto.Lines)
        {
            var product = await _productRepository.GetByIdAsync(lineDto.ProductId);
            if (product == null) continue;

            var line = new PurchaseOrderLine
            {
                Id = Guid.NewGuid(),
                PurchaseOrderId = order.Id,
                ProductId = lineDto.ProductId,
                Quantity = lineDto.QuantityOrdered,
                QuantityReceived = 0,
                UnitPrice = lineDto.UnitPrice,
                TaxRate = lineDto.TaxRate,
                Notes = lineDto.Notes
            };
            lines.Add(line);
        }

        order.Lines = lines;
        order.SubTotal = lines.Sum(l => l.Quantity * l.UnitPrice);
        order.TaxAmount = lines.Sum(l => l.Quantity * l.UnitPrice * l.TaxRate);
        order.Total = order.SubTotal + order.TaxAmount + order.ShippingCost;

        await _poRepository.AddAsync(order);

        return MapToDto(order);
    }

    public async Task<PurchaseOrderDto> UpdatePurchaseOrderStatusAsync(Guid id, string status, string? approvedBy = null)
    {
        var order = await _poRepository.GetByIdAsync(id);
        if (order == null) throw new Exception("Purchase order not found");

        var statusEnum = Enum.Parse<PurchaseOrderStatus>(status, true);
        order.Status = statusEnum;
        order.UpdatedAt = DateTime.UtcNow;

        if (statusEnum == PurchaseOrderStatus.Approved && !string.IsNullOrEmpty(approvedBy))
        {
            order.ApprovedBy = approvedBy;
            order.ApprovedAt = DateTime.UtcNow;
        }

        await _poRepository.UpdateAsync(order);

        return MapToDto(order);
    }

    public async Task<PurchaseOrderDto> ReceivePurchaseOrderAsync(Guid id, Dictionary<Guid, int> lineQuantities)
    {
        var order = await _poRepository.GetByIdAsync(id);
        if (order == null) throw new Exception("Purchase order not found");

        if (order.Status != PurchaseOrderStatus.Approved && order.Status != PurchaseOrderStatus.Sent && order.Status != PurchaseOrderStatus.PartiallyReceived)
        {
            throw new Exception("Purchase order must be approved or sent to receive");
        }

        foreach (var kvp in lineQuantities)
        {
            var line = order.Lines.FirstOrDefault(l => l.Id == kvp.Key);
            if (line == null) continue;

            var quantityToReceive = kvp.Value;
            if (quantityToReceive <= 0) continue;

            line.QuantityReceived += quantityToReceive;

            // Create stock item update
            var stockItem = await _stockItemRepository.GetByProductAndWarehouseAsync(line.ProductId, order.WarehouseId);
            if (stockItem == null)
            {
                stockItem = new StockItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = line.ProductId,
                    WarehouseId = order.WarehouseId,
                    QuantityOnHand = quantityToReceive,
                    QuantityAllocated = 0,
                    AverageCost = line.UnitPrice,
                    LastStockDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                await _stockItemRepository.AddAsync(stockItem);
            }
            else
            {
                stockItem.QuantityOnHand += quantityToReceive;
                stockItem.LastStockDate = DateTime.UtcNow;
                stockItem.UpdatedAt = DateTime.UtcNow;
                await _stockItemRepository.UpdateAsync(stockItem);
            }

            // Create transaction record
            var transaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductId = line.ProductId,
                WarehouseId = order.WarehouseId,
                Type = TransactionType.PurchaseOrder,
                Quantity = quantityToReceive,
                UnitCost = line.UnitPrice,
                ReferenceType = "PO",
                ReferenceNumber = order.OrderNumber,
                ReferenceId = order.Id,
                Notes = $"Received from PO {order.OrderNumber}",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await _transactionRepository.AddAsync(transaction);
        }

        // Update order status
        var allLinesFullyReceived = order.Lines.All(l => l.QuantityReceived >= l.Quantity);
        var anyLinesPartiallyReceived = order.Lines.Any(l => l.QuantityReceived > 0);

        if (allLinesFullyReceived)
        {
            order.Status = PurchaseOrderStatus.Received;
            order.ActualDeliveryDate = DateTime.UtcNow;
        }
        else if (anyLinesPartiallyReceived)
        {
            order.Status = PurchaseOrderStatus.PartiallyReceived;
        }

        order.UpdatedAt = DateTime.UtcNow;
        await _poRepository.UpdateAsync(order);

        return MapToDto(order);
    }

    public async Task DeletePurchaseOrderAsync(Guid id)
    {
        var order = await _poRepository.GetByIdAsync(id);
        if (order == null) throw new Exception("Purchase order not found");

        if (order.Status != PurchaseOrderStatus.Draft)
        {
            throw new Exception("Only draft purchase orders can be deleted");
        }

        await _poRepository.DeleteAsync(id);
    }

    private async Task<string> GenerateOrderNumber()
    {
        var lastOrder = (await _poRepository.GetAllAsync()).OrderByDescending(o => o.CreatedAt).FirstOrDefault();
        var lastNumber = 0;

        if (lastOrder != null && lastOrder.OrderNumber.StartsWith("PO"))
        {
            var numberPart = lastOrder.OrderNumber.Substring(2);
            int.TryParse(numberPart, out lastNumber);
        }

        return $"PO{(lastNumber + 1):D6}";
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        SupplierId = order.SupplierId,
        SupplierName = order.Supplier?.Name ?? "",
        WarehouseId = order.WarehouseId,
        WarehouseName = order.Warehouse?.Name ?? "",
        OrderDate = order.OrderDate,
        ExpectedDeliveryDate = order.ExpectedDeliveryDate,
        ActualDeliveryDate = order.ActualDeliveryDate,
        Status = order.Status.ToString(),
        SubTotal = order.SubTotal,
        TaxAmount = order.TaxAmount,
        ShippingCost = order.ShippingCost,
        Total = order.Total,
        Notes = order.Notes,
        Lines = order.Lines.Select(l => new PurchaseOrderLineDto
        {
            Id = l.Id,
            ProductId = l.ProductId,
            ProductName = l.Product?.Name ?? "",
            ProductSKU = l.Product?.SKU ?? "",
            QuantityOrdered = l.Quantity,
            QuantityReceived = l.QuantityReceived,
            UnitPrice = l.UnitPrice,
            TaxRate = l.TaxRate,
            LineTotal = l.LineTotal,
            Notes = l.Notes
        }).ToList()
    };
}
