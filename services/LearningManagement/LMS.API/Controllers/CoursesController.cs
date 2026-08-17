using LMS.Application.Features.Courses.Commands;
using LMS.Application.Features.Courses.Queries;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ILogger<CoursesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool publishedOnly = false, [FromQuery] string? authorId = null)
    {
        var query = new ListCoursesQuery
        {
            PublishedOnly = publishedOnly,
            AuthorId = authorId
        };

        var handler = HttpContext.RequestServices.GetRequiredService<ListCoursesQueryHandler>();
        var result = await handler.HandleAsync(query);

        if (result.Success)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetCourseQuery { CourseId = id };
        var handler = HttpContext.RequestServices.GetRequiredService<GetCourseQueryHandler>();
        var result = await handler.HandleAsync(query);

        if (result.Success)
            return Ok(result.Data);

        return NotFound(result.ErrorMessage);
    }

    [HttpGet("{id}/builder")]
    public async Task<IActionResult> GetForBuilder(Guid id)
    {
        var query = new GetCourseBuilderQuery { CourseId = id };
        var handler = HttpContext.RequestServices.GetRequiredService<GetCourseBuilderQueryHandler>();
        var result = await handler.HandleAsync(query);

        if (result.Success)
            return Ok(result.Data);

        return NotFound(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseCommand command)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<CreateCourseCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseCommand command)
    {
        if (id != command.CourseId)
            return BadRequest("Course ID mismatch");

        var handler = HttpContext.RequestServices.GetRequiredService<UpdateCourseCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var command = new PublishCourseCommand { CourseId = id };
        var handler = HttpContext.RequestServices.GetRequiredService<PublishCourseCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCourseCommand { CourseId = id };
        var handler = HttpContext.RequestServices.GetRequiredService<DeleteCourseCommandHandler>();
        var result = await handler.HandleAsync(command);

        if (result.Success)
            return NoContent();

        return BadRequest(result.ErrorMessage);
    }
}
