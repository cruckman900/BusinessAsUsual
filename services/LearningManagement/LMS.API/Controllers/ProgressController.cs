using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Core.Events;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly LMSDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(LMSDbContext context, IEventBus eventBus, ILogger<ProgressController> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>
    /// Get progress for a user on a specific course
    /// </summary>
    [HttpGet("{userId}/{courseId}")]
    public async Task<ActionResult<LearnerProgressDto>> GetProgress(string userId, Guid courseId)
    {
        var progress = await _context.DetailedLearnerProgress
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);

        if (progress == null)
        {
            return NotFound();
        }

        var dto = new LearnerProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            CourseId = progress.CourseId,
            CourseTitle = progress.Course?.Title ?? "",
            PercentComplete = progress.PercentComplete,
            StartedDate = progress.StartedDate,
            LastAccessedDate = progress.LastAccessedDate,
            CompletionDate = progress.CompletionDate,
            Score = progress.Score,
            TimeSpentMinutes = progress.TimeSpentMinutes,
            CurrentModule = progress.CurrentModule,
            Attempts = progress.Attempts,
            IsInProgress = progress.IsInProgress,
            IsCompleted = progress.IsCompleted
        };

        return Ok(dto);
    }

    /// <summary>
    /// Get all progress records for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<LearnerProgressDto>>> GetUserProgress(string userId)
    {
        var progressRecords = await _context.DetailedLearnerProgress
            .Include(p => p.Course)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.LastAccessedDate)
            .Select(p => new LearnerProgressDto
            {
                Id = p.Id,
                UserId = p.UserId,
                CourseId = p.CourseId,
                CourseTitle = p.Course != null ? p.Course.Title : "",
                PercentComplete = p.PercentComplete,
                StartedDate = p.StartedDate,
                LastAccessedDate = p.LastAccessedDate,
                CompletionDate = p.CompletionDate,
                Score = p.Score,
                TimeSpentMinutes = p.TimeSpentMinutes,
                CurrentModule = p.CurrentModule,
                Attempts = p.Attempts,
                IsInProgress = p.StartedDate.HasValue && !p.CompletionDate.HasValue,
                IsCompleted = p.CompletionDate.HasValue
            })
            .ToListAsync();

        return Ok(progressRecords);
    }

    /// <summary>
    /// Update progress for a user on a course
    /// </summary>
    [HttpPut("{userId}/{courseId}")]
    public async Task<ActionResult<LearnerProgressDto>> UpdateProgress(
        string userId,
        Guid courseId,
        [FromBody] UpdateProgressRequest request)
    {
        var progress = await _context.DetailedLearnerProgress
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);

        if (progress == null)
        {
            // Create new progress record
            progress = new DetailedLearnerProgress
            {
                UserId = userId,
                CourseId = courseId,
                StartedDate = DateTime.UtcNow
            };
            _context.DetailedLearnerProgress.Add(progress);
        }

        progress.PercentComplete = request.PercentComplete;
        progress.LastAccessedDate = DateTime.UtcNow;
        progress.CurrentModule = request.CurrentModule;
        progress.TimeSpentMinutes += request.AdditionalTimeSpentMinutes;

        // Update assignment status if exists
        var assignment = await _context.CourseAssignments
            .FirstOrDefaultAsync(a => a.UserId == userId && a.CourseId == courseId);

        if (assignment != null && assignment.Status == CourseAssignmentStatus.Assigned)
        {
            assignment.Status = CourseAssignmentStatus.InProgress;
        }

        await _context.SaveChangesAsync();

        var dto = new LearnerProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            CourseId = progress.CourseId,
            CourseTitle = progress.Course?.Title ?? "",
            PercentComplete = progress.PercentComplete,
            StartedDate = progress.StartedDate,
            LastAccessedDate = progress.LastAccessedDate,
            CompletionDate = progress.CompletionDate,
            Score = progress.Score,
            TimeSpentMinutes = progress.TimeSpentMinutes,
            CurrentModule = progress.CurrentModule,
            Attempts = progress.Attempts,
            IsInProgress = progress.IsInProgress,
            IsCompleted = progress.IsCompleted
        };

        return Ok(dto);
    }

    /// <summary>
    /// Mark a course as complete and issue certificate
    /// </summary>
    [HttpPost("{userId}/{courseId}/complete")]
    public async Task<ActionResult<CompletionResult>> CompleteCourse(
        string userId,
        Guid courseId,
        [FromBody] CompleteCourseRequest request)
    {
        var progress = await _context.DetailedLearnerProgress
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);

        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
        {
            return NotFound("Course not found");
        }

        if (progress == null)
        {
            progress = new DetailedLearnerProgress
            {
                UserId = userId,
                CourseId = courseId,
                StartedDate = DateTime.UtcNow.AddHours(-1),
                Course = course
            };
            _context.DetailedLearnerProgress.Add(progress);
        }

        progress.PercentComplete = 100;
        progress.CompletionDate = DateTime.UtcNow;
        progress.LastAccessedDate = DateTime.UtcNow;
        progress.Score = request.Score;
        progress.Attempts++;

        // Update assignment status
        var assignment = await _context.CourseAssignments
            .FirstOrDefaultAsync(a => a.UserId == userId && a.CourseId == courseId);

        if (assignment != null)
        {
            assignment.Status = CourseAssignmentStatus.Completed;
        }

        // Generate certificate if course issues certificates
        Certificate? certificate = null;
        if (course.IssuesCertificate && request.Score >= course.PassingScore)
        {
            certificate = new Certificate
            {
                CertificateNumber = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                UserId = userId,
                CourseId = courseId,
                IssuedDate = DateTime.UtcNow,
                ExpirationDate = course.CertificateValidityDays.HasValue
                    ? DateTime.UtcNow.AddDays(course.CertificateValidityDays.Value)
                    : null,
                Score = request.Score,
                Status = CertificateStatus.Active
            };

            _context.Certificates.Add(certificate);
        }

        await _context.SaveChangesAsync();

        // Publish course completion event for HR integration
        await _eventBus.PublishAsync(new CourseCompletedEvent
        {
            UserId = userId,
            CourseId = courseId,
            CourseName = course.Title,
            CompletionDate = progress.CompletionDate.Value,
            Score = request.Score ?? 0,
            CertificateNumber = certificate?.CertificateNumber,
            TimeSpentMinutes = progress.TimeSpentMinutes
        });

        _logger.LogInformation("Course {CourseId} completed by user {UserId} with score {Score}", 
            courseId, userId, request.Score);

        return Ok(new CompletionResult
        {
            Success = true,
            CertificateIssued = certificate != null,
            CertificateNumber = certificate?.CertificateNumber,
            CompletionDate = progress.CompletionDate.Value,
            Score = request.Score
        });
    }

    /// <summary>
    /// Get all certificates for a user
    /// </summary>
    [HttpGet("certificates/{userId}")]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> GetUserCertificates(string userId)
    {
        var certificates = await _context.Certificates
            .Include(c => c.Course)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.IssuedDate)
            .Select(c => new CertificateDto
            {
                Id = c.Id,
                CertificateNumber = c.CertificateNumber,
                UserId = c.UserId,
                CourseId = c.CourseId,
                CourseTitle = c.Course != null ? c.Course.Title : "",
                IssuedDate = c.IssuedDate,
                ExpirationDate = c.ExpirationDate,
                Score = c.Score,
                Status = c.Status.ToString(),
                IssuedBy = c.IssuedBy
            })
            .ToListAsync();

        return Ok(certificates);
    }
}

// DTOs
public class LearnerProgressDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public decimal PercentComplete { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? LastAccessedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal? Score { get; set; }
    public int TimeSpentMinutes { get; set; }
    public string? CurrentModule { get; set; }
    public int Attempts { get; set; }
    public bool IsInProgress { get; set; }
    public bool IsCompleted { get; set; }
}

public class UpdateProgressRequest
{
    public decimal PercentComplete { get; set; }
    public string? CurrentModule { get; set; }
    public int AdditionalTimeSpentMinutes { get; set; }
}

public class CompleteCourseRequest
{
    public decimal? Score { get; set; }
}

public class CompletionResult
{
    public bool Success { get; set; }
    public bool CertificateIssued { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime CompletionDate { get; set; }
    public decimal? Score { get; set; }
}

public class CertificateDto
{
    public Guid Id { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal? Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public string IssuedBy { get; set; } = string.Empty;
}

// Event for HR integration
public class CourseCompletedEvent : IntegrationEvent
{
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public decimal Score { get; set; }
    public string? CertificateNumber { get; set; }
    public int TimeSpentMinutes { get; set; }
}
