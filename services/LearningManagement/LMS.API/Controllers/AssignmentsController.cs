using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly LMSDbContext _context;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(LMSDbContext context, ILogger<AssignmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all assignments for a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<CourseAssignmentDto>>> GetUserAssignments(string userId)
    {
        var assignments = await _context.CourseAssignments
            .Include(a => a.Course)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AssignedDate)
            .Select(a => new CourseAssignmentDto
            {
                Id = a.Id,
                UserId = a.UserId,
                CourseId = a.CourseId,
                CourseTitle = a.Course != null ? a.Course.Title : "",
                CourseDescription = a.Course != null ? a.Course.Description : "",
                CourseCategory = a.Course != null ? a.Course.Category : "",
                CourseDifficulty = a.Course != null ? a.Course.Difficulty.ToString() : "",
                AssignedBy = a.AssignedBy,
                AssignedDate = a.AssignedDate,
                DueDate = a.DueDate,
                Status = a.Status.ToString(),
                IsMandatory = a.IsMandatory,
                Notes = a.Notes
            })
            .ToListAsync();

        return Ok(assignments);
    }

    /// <summary>
    /// Get all assignments (admin)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseAssignmentDto>>> GetAllAssignments(
        [FromQuery] string? status = null,
        [FromQuery] string? userId = null,
        [FromQuery] Guid? courseId = null)
    {
        var query = _context.CourseAssignments
            .Include(a => a.Course)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<CourseAssignmentStatus>(status, true, out var statusEnum))
        {
            query = query.Where(a => a.Status == statusEnum);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(a => a.UserId == userId);
        }

        if (courseId.HasValue)
        {
            query = query.Where(a => a.CourseId == courseId.Value);
        }

        var assignments = await query
            .OrderByDescending(a => a.AssignedDate)
            .Select(a => new CourseAssignmentDto
            {
                Id = a.Id,
                UserId = a.UserId,
                CourseId = a.CourseId,
                CourseTitle = a.Course != null ? a.Course.Title : "",
                CourseDescription = a.Course != null ? a.Course.Description : "",
                CourseCategory = a.Course != null ? a.Course.Category : "",
                CourseDifficulty = a.Course != null ? a.Course.Difficulty.ToString() : "",
                AssignedBy = a.AssignedBy,
                AssignedDate = a.AssignedDate,
                DueDate = a.DueDate,
                Status = a.Status.ToString(),
                IsMandatory = a.IsMandatory,
                Notes = a.Notes
            })
            .ToListAsync();

        return Ok(assignments);
    }

    /// <summary>
    /// Assign a course to one or more users
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<List<CourseAssignmentDto>>> AssignCourse([FromBody] CreateAssignmentRequest request)
    {
        // Verify course exists
        var course = await _context.Courses.FindAsync(request.CourseId);
        if (course == null)
        {
            return NotFound($"Course with ID {request.CourseId} not found");
        }

        var assignments = new List<CourseAssignment>();

        foreach (var userId in request.UserIds)
        {
            // Check if already assigned
            var existing = await _context.CourseAssignments
                .FirstOrDefaultAsync(a => a.UserId == userId && a.CourseId == request.CourseId && a.Status != CourseAssignmentStatus.Cancelled);

            if (existing != null)
            {
                _logger.LogWarning("User {UserId} already has assignment for course {CourseId}", userId, request.CourseId);
                continue;
            }

            var assignment = new CourseAssignment
            {
                UserId = userId,
                CourseId = request.CourseId,
                AssignedBy = request.AssignedBy ?? "admin",
                AssignedDate = DateTime.UtcNow,
                DueDate = request.DueDate,
                Status = CourseAssignmentStatus.Assigned,
                IsMandatory = request.IsMandatory,
                Notes = request.Notes
            };

            assignments.Add(assignment);
        }

        if (assignments.Any())
        {
            await _context.CourseAssignments.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();
        }

        var dtos = assignments.Select(a => new CourseAssignmentDto
        {
            Id = a.Id,
            UserId = a.UserId,
            CourseId = a.CourseId,
            CourseTitle = course.Title,
            CourseDescription = course.Description,
            CourseCategory = course.Category,
            CourseDifficulty = course.Difficulty.ToString(),
            AssignedBy = a.AssignedBy,
            AssignedDate = a.AssignedDate,
            DueDate = a.DueDate,
            Status = a.Status.ToString(),
            IsMandatory = a.IsMandatory,
            Notes = a.Notes
        }).ToList();

        return CreatedAtAction(nameof(GetAllAssignments), dtos);
    }

    /// <summary>
    /// Cancel an assignment
    /// </summary>
    [HttpDelete("{assignmentId}")]
    public async Task<ActionResult> CancelAssignment(Guid assignmentId)
    {
        var assignment = await _context.CourseAssignments.FindAsync(assignmentId);
        if (assignment == null)
        {
            return NotFound();
        }

        assignment.Status = CourseAssignmentStatus.Cancelled;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// DTOs
public class CourseAssignmentDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseDescription { get; set; } = string.Empty;
    public string CourseCategory { get; set; } = string.Empty;
    public string CourseDifficulty { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public string? Notes { get; set; }
}

public class CreateAssignmentRequest
{
    public Guid CourseId { get; set; }
    public List<string> UserIds { get; set; } = new();
    public string? AssignedBy { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsMandatory { get; set; }
    public string? Notes { get; set; }
}
