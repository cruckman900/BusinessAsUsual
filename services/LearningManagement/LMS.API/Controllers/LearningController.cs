using LMS.Application.Features.Learning.Commands;
using LMS.Application.Features.Learning.Queries;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningController : ControllerBase
{
    private readonly ILogger<LearningController> _logger;

    public LearningController(ILogger<LearningController> logger)
    {
        _logger = logger;
    }

    [HttpGet("my-courses")]
    public async Task<IActionResult> GetMyCourses([FromQuery] string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            return BadRequest("Employee ID is required");

        var query = new GetMyCoursesQuery { EmployeeId = employeeId };
        var handler = HttpContext.RequestServices.GetRequiredService<GetMyCoursesQueryHandler>();
        var result = await handler.HandleAsync(query);

        if (result.Success)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpGet("progress")]
    public async Task<IActionResult> GetProgress([FromQuery] string employeeId, [FromQuery] Guid courseId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            return BadRequest("Employee ID is required");

        if (courseId == Guid.Empty)
            return BadRequest("Course ID is required");

        var query = new GetProgressQuery 
        { 
            EmployeeId = employeeId,
            CourseId = courseId
        };
        var handler = HttpContext.RequestServices.GetRequiredService<GetProgressQueryHandler>();
        var result = await handler.HandleAsync(query);

        if (result.Success)
            return Ok(result.Data);

        return NotFound(result.ErrorMessage);
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] EnrollInCourseCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<EnrollInCourseCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartCourse([FromBody] StartCourseCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<StartCourseCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpPut("progress")]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<UpdateProgressCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteCourse([FromBody] CompleteCourseCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<CompleteCourseCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }
}
