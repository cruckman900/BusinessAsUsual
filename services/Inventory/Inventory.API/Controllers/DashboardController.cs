using Inventory.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    public DashboardController(
        IProductRepository productRepository,
        IStockItemRepository stockItemRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        IWarehouseRepository warehouseRepository)
    {
        _productRepository = productRepository;
        _stockItemRepository = stockItemRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _warehouseRepository = warehouseRepository;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetSummary()
    {
        var products = await _productRepository.GetAllAsync();
        var stockItems = await _stockItemRepository.GetAllAsync();
        var purchaseOrders = await _purchaseOrderRepository.GetAllAsync();
        var warehouses = await _warehouseRepository.GetAllAsync();
        var lowStockProducts = await _productRepository.GetLowStockAsync();

        var totalInventoryValue = stockItems.Sum(s => s.QuantityOnHand * s.AverageCost);
        var totalOnHand = stockItems.Sum(s => s.QuantityOnHand);

        return Ok(new
        {
            TotalProducts = products.Count(),
            TotalWarehouses = warehouses.Count(),
            TotalInventoryValue = totalInventoryValue,
            TotalItemsInStock = totalOnHand,
            LowStockCount = lowStockProducts.Count(),
            ActivePurchaseOrders = purchaseOrders.Count(po => po.Status != Domain.Entities.PurchaseOrderStatus.Received &&
                                                               po.Status != Domain.Entities.PurchaseOrderStatus.Cancelled)
        });
    }
}
