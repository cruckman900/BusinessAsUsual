using Sales.Application.DTOs;
using Sales.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/sales/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(string id, [FromBody] UpdateOrderDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        try
        {
            var order = await _orderService.UpdateOrderAsync(dto);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<OrderDto>> Confirm(string id)
    {
        try
        {
            var order = await _orderService.ConfirmOrderAsync(id);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/ship")]
    public async Task<ActionResult<OrderDto>> Ship(string id, [FromBody] ShipOrderRequest request)
    {
        try
        {
            var order = await _orderService.ShipOrderAsync(id, request.TrackingNumber, request.ShippedDate);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/deliver")]
    public async Task<ActionResult<OrderDto>> Deliver(string id, [FromBody] DeliverOrderRequest? request = null)
    {
        try
        {
            var order = await _orderService.DeliverOrderAsync(id, request?.DeliveredDate);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(string id)
    {
        try
        {
            var order = await _orderService.CancelOrderAsync(id);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("payments")]
    public async Task<ActionResult<OrderPaymentDto>> AddPayment([FromBody] AddOrderPaymentDto dto)
    {
        try
        {
            var payment = await _orderService.AddPaymentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.OrderId }, payment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record ShipOrderRequest(string TrackingNumber, DateTime? ShippedDate = null);
public record DeliverOrderRequest(DateTime? DeliveredDate = null);
