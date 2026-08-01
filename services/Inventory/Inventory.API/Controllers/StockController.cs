using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<StockItemDto>>> GetAllStockItems()
    {
        var items = await _stockService.GetAllStockItemsAsync();
        return Ok(items);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<StockSummaryDto>>> GetStockSummary()
    {
        var summary = await _stockService.GetStockSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<StockItemDto>>> GetStockByWarehouse(Guid warehouseId)
    {
        var items = await _stockService.GetStockByWarehouseAsync(warehouseId);
        return Ok(items);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<IEnumerable<InventoryTransactionDto>>> GetRecentTransactions([FromQuery] int count = 100)
    {
        var transactions = await _stockService.GetRecentTransactionsAsync(count);
        return Ok(transactions);
    }

    [HttpPost("adjustment")]
    public async Task<ActionResult<InventoryTransactionDto>> CreateStockAdjustment([FromBody] CreateStockAdjustmentDto dto)
    {
        try
        {
            var transaction = await _stockService.CreateStockAdjustmentAsync(dto);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("transfer")]
    public async Task<ActionResult<IEnumerable<InventoryTransactionDto>>> CreateStockTransfer([FromBody] CreateStockTransferDto dto)
    {
        try
        {
            var transactions = await _stockService.CreateStockTransferAsync(dto);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
