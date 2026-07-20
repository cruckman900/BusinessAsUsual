using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        => Ok(await _paymentService.GetAllPaymentsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(string id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpGet("by-invoice/{invoiceId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByInvoice(string invoiceId)
        => Ok(await _paymentService.GetPaymentsByInvoiceAsync(invoiceId));

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Record(RecordPaymentRequest request)
    {
        var payment = await _paymentService.RecordPaymentAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _paymentService.DeletePaymentAsync(id);
        return NoContent();
    }
}
