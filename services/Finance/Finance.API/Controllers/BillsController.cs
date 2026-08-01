using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BillDto>>> GetAll()
        => Ok(await _billService.GetAllBillsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<BillDto>> GetById(string id)
    {
        var bill = await _billService.GetBillByIdAsync(id);
        return bill is null ? NotFound() : Ok(bill);
    }

    [HttpGet("by-vendor/{vendorId}")]
    public async Task<ActionResult<IEnumerable<BillDto>>> GetByVendor(string vendorId)
        => Ok(await _billService.GetBillsByVendorAsync(vendorId));

    [HttpPost]
    public async Task<ActionResult<BillDto>> Create(CreateBillRequest request)
    {
        var bill = await _billService.CreateBillAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = bill.Id }, bill);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BillDto>> Update(string id, UpdateBillRequest request)
    {
        try
        {
            return Ok(await _billService.UpdateBillAsync(id, request));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _billService.DeleteBillAsync(id);
        return NoContent();
    }
}
