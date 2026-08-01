using Inventory.Application.DTOs;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class StockService
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IInventoryTransactionRepository _transactionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    public StockService(
        IStockItemRepository stockItemRepository,
        IInventoryTransactionRepository transactionRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository)
    {
        _stockItemRepository = stockItemRepository;
        _transactionRepository = transactionRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<IEnumerable<StockItemDto>> GetAllStockItemsAsync()
    {
        var stockItems = await _stockItemRepository.GetAllAsync();
        return stockItems.Select(MapToDto);
    }

    public async Task<IEnumerable<StockSummaryDto>> GetStockSummaryAsync()
    {
        var stockItems = await _stockItemRepository.GetAllAsync();
        var grouped = stockItems.GroupBy(s => s.ProductId);

        var summaries = new List<StockSummaryDto>();
        foreach (var group in grouped)
        {
            var items = group.ToList();
            var product = items.First().Product;

            summaries.Add(new StockSummaryDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSKU = product.SKU,
                TotalOnHand = items.Sum(i => i.QuantityOnHand),
                TotalAllocated = items.Sum(i => i.QuantityAllocated),
                TotalAvailable = items.Sum(i => i.QuantityAvailable),
                TotalValue = items.Sum(i => i.QuantityOnHand * i.AverageCost),
                LocationBreakdown = items.Select(MapToDto).ToList()
            });
        }

        return summaries;
    }

    public async Task<IEnumerable<StockItemDto>> GetStockByWarehouseAsync(Guid warehouseId)
    {
        var stockItems = await _stockItemRepository.GetByWarehouseIdAsync(warehouseId);
        return stockItems.Select(MapToDto);
    }

    public async Task<IEnumerable<InventoryTransactionDto>> GetRecentTransactionsAsync(int count = 100)
    {
        var transactions = await _transactionRepository.GetAllAsync();
        return transactions.OrderByDescending(t => t.TransactionDate).Take(count).Select(MapTransactionToDto);
    }

    public async Task<InventoryTransactionDto> CreateStockAdjustmentAsync(CreateStockAdjustmentDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null) throw new Exception("Product not found");

        var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
        if (warehouse == null) throw new Exception("Warehouse not found");

        var transaction = new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            WarehouseId = dto.WarehouseId,
            BinLocationId = dto.BinLocationId,
            Type = TransactionType.StockAdjustment,
            Quantity = dto.Quantity,
            UnitCost = product.Cost,
            ReferenceType = "Adjustment",
            Notes = dto.Notes,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _transactionRepository.AddAsync(transaction);
        await UpdateStockItem(dto.ProductId, dto.WarehouseId, dto.BinLocationId, dto.Quantity, product.Cost);

        return MapTransactionToDto(transaction);
    }

    public async Task<IEnumerable<InventoryTransactionDto>> CreateStockTransferAsync(CreateStockTransferDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null) throw new Exception("Product not found");

        var results = new List<InventoryTransaction>();

        // Outbound transaction
        var outbound = new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            WarehouseId = dto.FromWarehouseId,
            BinLocationId = dto.FromBinLocationId,
            Type = TransactionType.Transfer,
            Quantity = -dto.Quantity,
            UnitCost = product.Cost,
            ReferenceType = "Transfer-Out",
            Notes = dto.Notes,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = "System"
        };
        await _transactionRepository.AddAsync(outbound);
        await UpdateStockItem(dto.ProductId, dto.FromWarehouseId, dto.FromBinLocationId, -dto.Quantity, product.Cost);
        results.Add(outbound);

        // Inbound transaction
        var inbound = new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            WarehouseId = dto.ToWarehouseId,
            BinLocationId = dto.ToBinLocationId,
            Type = TransactionType.Transfer,
            Quantity = dto.Quantity,
            UnitCost = product.Cost,
            ReferenceType = "Transfer-In",
            ReferenceId = outbound.Id,
            Notes = dto.Notes,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = "System"
        };
        await _transactionRepository.AddAsync(inbound);
        await UpdateStockItem(dto.ProductId, dto.ToWarehouseId, dto.ToBinLocationId, dto.Quantity, product.Cost);
        results.Add(inbound);

        return results.Select(MapTransactionToDto);
    }

    private async Task UpdateStockItem(Guid productId, Guid warehouseId, Guid? binLocationId, int quantityChange, decimal unitCost)
    {
        var stockItem = await _stockItemRepository.GetByProductAndWarehouseAsync(productId, warehouseId);

        if (stockItem == null)
        {
            stockItem = new StockItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                WarehouseId = warehouseId,
                BinLocationId = binLocationId,
                QuantityOnHand = quantityChange,
                QuantityAllocated = 0,
                AverageCost = unitCost,
                LastStockDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await _stockItemRepository.AddAsync(stockItem);
        }
        else
        {
            stockItem.QuantityOnHand += quantityChange;
            stockItem.LastStockDate = DateTime.UtcNow;
            stockItem.UpdatedAt = DateTime.UtcNow;
            await _stockItemRepository.UpdateAsync(stockItem);
        }
    }

    private StockItemDto MapToDto(StockItem item) => new()
    {
        Id = item.Id,
        ProductId = item.ProductId,
        ProductName = item.Product?.Name ?? "",
        ProductSKU = item.Product?.SKU ?? "",
        WarehouseId = item.WarehouseId,
        WarehouseName = item.Warehouse?.Name ?? "",
        BinLocationId = item.BinLocationId,
        BinLocationCode = item.BinLocation?.Code,
        QuantityOnHand = item.QuantityOnHand,
        QuantityAllocated = item.QuantityAllocated,
        QuantityAvailable = item.QuantityAvailable,
        AverageCost = item.AverageCost,
        LastStockDate = item.LastStockDate
    };

    private InventoryTransactionDto MapTransactionToDto(InventoryTransaction transaction) => new()
    {
        Id = transaction.Id,
        ProductId = transaction.ProductId,
        ProductName = transaction.Product?.Name ?? "",
        ProductSKU = transaction.Product?.SKU ?? "",
        WarehouseId = transaction.WarehouseId,
        WarehouseName = transaction.Warehouse?.Name ?? "",
        TransactionType = transaction.Type.ToString(),
        Quantity = transaction.Quantity,
        UnitCost = transaction.UnitCost,
        TotalCost = transaction.TotalCost,
        RunningBalance = transaction.RunningBalance,
        ReferenceNumber = transaction.ReferenceNumber,
        Notes = transaction.Notes,
        TransactionDate = transaction.TransactionDate,
        CreatedBy = transaction.CreatedBy
    };
}
