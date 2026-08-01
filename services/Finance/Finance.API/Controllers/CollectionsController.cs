using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _collectionService;

    public CollectionsController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    [HttpGet("aging-report")]
    public async Task<ActionResult<AgingReportDto>> GetAgingReport([FromQuery] DateTime? asOfDate = null)
        => Ok(await _collectionService.GenerateAgingReportAsync(asOfDate));

    [HttpGet("activities")]
    public async Task<ActionResult<IEnumerable<CollectionActivityDto>>> GetAllActivities()
        => Ok(await _collectionService.GetAllActivitiesAsync());

    [HttpGet("activities/{id}")]
    public async Task<ActionResult<CollectionActivityDto>> GetActivityById(string id)
    {
        var activity = await _collectionService.GetActivityByIdAsync(id);
        return activity is null ? NotFound() : Ok(activity);
    }

    [HttpGet("activities/by-invoice/{invoiceId}")]
    public async Task<ActionResult<IEnumerable<CollectionActivityDto>>> GetActivitiesByInvoice(string invoiceId)
        => Ok(await _collectionService.GetActivitiesByInvoiceAsync(invoiceId));

    [HttpGet("activities/by-customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<CollectionActivityDto>>> GetActivitiesByCustomer(string customerId)
        => Ok(await _collectionService.GetActivitiesByCustomerAsync(customerId));

    [HttpPost("activities")]
    public async Task<ActionResult<CollectionActivityDto>> CreateActivity(CreateCollectionActivityRequest request)
    {
        var activity = await _collectionService.CreateActivityAsync(request);
        return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
    }

    [HttpPut("activities/{id}")]
    public async Task<ActionResult<CollectionActivityDto>> UpdateActivity(string id, UpdateCollectionActivityRequest request)
    {
        try
        {
            return Ok(await _collectionService.UpdateActivityAsync(id, request));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("activities/{id}")]
    public async Task<ActionResult> DeleteActivity(string id)
    {
        await _collectionService.DeleteActivityAsync(id);
        return NoContent();
    }
}
