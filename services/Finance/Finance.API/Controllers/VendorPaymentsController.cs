using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorPaymentsController : ControllerBase
{
    private readonly IVendorPaymentService _vendorPaymentService;

    public VendorPaymentsController(IVendorPaymentService vendorPaymentService)
    {
        _vendorPaymentService = vendorPaymentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VendorPaymentDto>>> GetAll()
        => Ok(await _vendorPaymentService.GetAllVendorPaymentsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<VendorPaymentDto>> GetById(string id)
    {
        var payment = await _vendorPaymentService.GetVendorPaymentByIdAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpGet("by-bill/{billId}")]
    public async Task<ActionResult<IEnumerable<VendorPaymentDto>>> GetByBill(string billId)
        => Ok(await _vendorPaymentService.GetVendorPaymentsByBillAsync(billId));

    [HttpPost]
    public async Task<ActionResult<VendorPaymentDto>> Record(RecordVendorPaymentRequest request)
    {
        var payment = await _vendorPaymentService.RecordVendorPaymentAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _vendorPaymentService.DeleteVendorPaymentAsync(id);
        return NoContent();
    }
}
