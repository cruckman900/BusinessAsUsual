using LMS.Application.Common;
using LMS.Application.Features.LearningPaths.Commands;
using LMS.Application.Features.LearningPaths.Queries;
using LMS.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/lms/learning-paths")]
// [Authorize] // Temporarily disabled for integrated Web app
public class LearningPathsController : ControllerBase
{
    private readonly ICommandHandler<CreateLearningPathCommand, Result<Guid>> _createPathHandler;
    private readonly IQueryHandler<GetLearningPathsQuery, Result<List<LearningPathDto>>> _getPathsHandler;
    private readonly ILogger<LearningPathsController> _logger;

    public LearningPathsController(
        ICommandHandler<CreateLearningPathCommand, Result<Guid>> createPathHandler,
        IQueryHandler<GetLearningPathsQuery, Result<List<LearningPathDto>>> getPathsHandler,
        ILogger<LearningPathsController> logger)
    {
        _createPathHandler = createPathHandler;
        _getPathsHandler = getPathsHandler;
        _logger = logger;
    }

    /// <summary>
    /// Get all learning paths
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<LearningPathDto>), 200)]
    public async Task<IActionResult> GetLearningPaths(
        [FromQuery] bool publishedOnly = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _getPathsHandler.HandleAsync(
            new GetLearningPathsQuery { PublishedOnly = publishedOnly },
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new learning path
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateLearningPath(
        [FromBody] CreateLearningPathRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";

        var result = await _createPathHandler.HandleAsync(
            new CreateLearningPathCommand
            {
                Request = request,
                CreatedBy = userId
            },
            cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to create learning path: {Error}", result.ErrorMessage);
            return BadRequest(new { error = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetLearningPaths), new { id = result.Data }, result.Data);
    }
}
