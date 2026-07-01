using CRM.Application.DTOs;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadsController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeadDto>>> GetAll()
    {
        var leads = await _leadService.GetAllLeadsAsync();
        return Ok(leads);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeadDto>> GetById(string id)
    {
        var lead = await _leadService.GetLeadByIdAsync(id);
        if (lead == null)
            return NotFound();
        return Ok(lead);
    }

    [HttpPost]
    public async Task<ActionResult<LeadDto>> Create(CreateLeadRequest request)
    {
        var lead = await _leadService.CreateLeadAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = lead.Id }, lead);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LeadDto>> Update(string id, UpdateLeadRequest request)
    {
        try
        {
            var lead = await _leadService.UpdateLeadAsync(id, request);
            return Ok(lead);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _leadService.DeleteLeadAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/convert")]
    public async Task<ActionResult<LeadDto>> ConvertToCustomer(string id)
    {
        try
        {
            var lead = await _leadService.ConvertLeadToCustomerAsync(id);
            return Ok(lead);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
