using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly PurchaseOrderService _poService;

    public PurchaseOrdersController(PurchaseOrderService poService)
    {
        _poService = poService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetAll()
    {
        var orders = await _poService.GetAllPurchaseOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseOrderDto>> GetById(Guid id)
    {
        var order = await _poService.GetPurchaseOrderByIdAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetBySupplier(Guid supplierId)
    {
        var orders = await _poService.GetPurchaseOrdersBySupplierAsync(supplierId);
        return Ok(orders);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetByStatus(string status)
    {
        var orders = await _poService.GetPurchaseOrdersByStatusAsync(status);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderDto dto)
    {
        try
        {
            var order = await _poService.CreatePurchaseOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<PurchaseOrderDto>> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        try
        {
            var order = await _poService.UpdatePurchaseOrderStatusAsync(id, dto.Status, dto.ApprovedBy);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/receive")]
    public async Task<ActionResult<PurchaseOrderDto>> ReceiveOrder(Guid id, [FromBody] ReceiveOrderDto dto)
    {
        try
        {
            var order = await _poService.ReceivePurchaseOrderAsync(id, dto.LineQuantities);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _poService.DeletePurchaseOrderAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
}

public class ReceiveOrderDto
{
    public Dictionary<Guid, int> LineQuantities { get; set; } = new();
}
