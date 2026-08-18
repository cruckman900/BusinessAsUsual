using LMS.Application.Common;
using LMS.Application.Features.Gamification.Queries;
using LMS.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/lms/gamification")]
// [Authorize] // Temporarily disabled for integrated Web app
public class GamificationController : ControllerBase
{
    private readonly IQueryHandler<GetGamificationStatsQuery, Result<GamificationStatsDto>> _getStatsHandler;
    private readonly ILogger<GamificationController> _logger;

    public GamificationController(
        IQueryHandler<GetGamificationStatsQuery, Result<GamificationStatsDto>> getStatsHandler,
        ILogger<GamificationController> logger)
    {
        _getStatsHandler = getStatsHandler;
        _logger = logger;
    }

    /// <summary>
    /// Get gamification stats for a learner
    /// </summary>
    [HttpGet("stats/{employeeId}")]
    [ProducesResponseType(typeof(GamificationStatsDto), 200)]
    public async Task<IActionResult> GetStats(
        string employeeId,
        CancellationToken cancellationToken = default)
    {
        var result = await _getStatsHandler.HandleAsync(
            new GetGamificationStatsQuery { EmployeeId = employeeId },
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get leaderboard
    /// </summary>
    [HttpGet("leaderboard")]
    [ProducesResponseType(typeof(List<LeaderboardEntryDto>), 200)]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement leaderboard query
        return Ok(new List<LeaderboardEntryDto>());
    }
}
