using CRM.Application.DTOs;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpportunitiesController : ControllerBase
{
    private readonly IOpportunityService _opportunityService;

    public OpportunitiesController(IOpportunityService opportunityService)
    {
        _opportunityService = opportunityService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OpportunityDto>>> GetAll()
    {
        var opportunities = await _opportunityService.GetAllOpportunitiesAsync();
        return Ok(opportunities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OpportunityDto>> GetById(string id)
    {
        var opportunity = await _opportunityService.GetOpportunityByIdAsync(id);
        if (opportunity == null)
            return NotFound();
        return Ok(opportunity);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OpportunityDto>>> GetByCustomer(string customerId)
    {
        var opportunities = await _opportunityService.GetOpportunitiesByCustomerAsync(customerId);
        return Ok(opportunities);
    }

    [HttpPost]
    public async Task<ActionResult<OpportunityDto>> Create(CreateOpportunityRequest request)
    {
        var opportunity = await _opportunityService.CreateOpportunityAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = opportunity.Id }, opportunity);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OpportunityDto>> Update(string id, UpdateOpportunityRequest request)
    {
        try
        {
            var opportunity = await _opportunityService.UpdateOpportunityAsync(id, request);
            return Ok(opportunity);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _opportunityService.DeleteOpportunityAsync(id);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(string id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest request)
    {
        var customer = await _customerService.CreateCustomerAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> Update(string id, UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _customerService.UpdateCustomerAsync(id, request);
            return Ok(customer);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _customerService.DeleteCustomerAsync(id);
        return NoContent();
    }
}
