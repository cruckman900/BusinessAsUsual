using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
        => Ok(await _invoiceService.GetAllInvoicesAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetById(string id)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
        return invoice is null ? NotFound() : Ok(invoice);
    }

    [HttpGet("by-customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetByCustomer(string customerId)
        => Ok(await _invoiceService.GetInvoicesByCustomerAsync(customerId));

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create(CreateInvoiceRequest request)
    {
        var invoice = await _invoiceService.CreateInvoiceAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<InvoiceDto>> Update(string id, UpdateInvoiceRequest request)
    {
        try
        {
            return Ok(await _invoiceService.UpdateInvoiceAsync(id, request));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/send")]
    public async Task<ActionResult<InvoiceDto>> Send(string id)
    {
        try
        {
            return Ok(await _invoiceService.SendInvoiceAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _invoiceService.DeleteInvoiceAsync(id);
        return NoContent();
    }
}
