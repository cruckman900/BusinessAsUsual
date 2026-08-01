using CRM.Application.DTOs;
using CRM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadScoringController : ControllerBase
{
    private readonly ILeadScoringService _scoringService;

    public LeadScoringController(ILeadScoringService scoringService)
    {
        _scoringService = scoringService;
    }

    [HttpGet("{leadId}")]
    public async Task<ActionResult<LeadScoreDto>> GetLeadScore(string leadId)
    {
        try
        {
            return Ok(await _scoringService.CalculateLeadScoreAsync(leadId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<LeadScoreDto>>> GetAllLeadScores()
        => Ok(await _scoringService.CalculateAllLeadScoresAsync());

    [HttpGet("hot-leads")]
    public async Task<ActionResult<IEnumerable<LeadDto>>> GetHotLeads()
        => Ok(await _scoringService.GetHotLeadsAsync());

    [HttpGet("config")]
    public async Task<ActionResult<LeadScoringConfigDto>> GetConfig()
        => Ok(await _scoringService.GetScoringConfigAsync());

    [HttpPut("config")]
    public async Task<ActionResult<LeadScoringConfigDto>> UpdateConfig(LeadScoringConfigDto config)
        => Ok(await _scoringService.UpdateScoringConfigAsync(config));
}
