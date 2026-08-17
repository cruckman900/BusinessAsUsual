using System.Net.Http.Json;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Web.Services;

/// <summary>
/// Service for interacting with the LMS API from the integrated BusinessAsUsual.Web module.
/// Provides methods for fetching courses, assignments, completions, and managing learner progress.
/// Falls back to empty results if LMS.API is unavailable.
/// </summary>
public interface ILMSService
{
    /// <summary>
    /// Gets all published courses from the LMS catalog.
    /// </summary>
    Task<IEnumerable<CourseDto>> GetPublishedCoursesAsync();

    /// <summary>
    /// Gets courses assigned to the current user.
    /// </summary>
    Task<IEnumerable<CourseDto>> GetMyCoursesAsync(string userId);

    /// <summary>
    /// Gets course completions/certificates for the current user.
    /// </summary>
    Task<IEnumerable<CompletionDto>> GetMyCompletionsAsync(string userId);

    /// <summary>
    /// Gets LMS admin dashboard statistics.
    /// </summary>
    Task<AdminStatsDto> GetAdminStatsAsync();

    /// <summary>
    /// Gets recent course activity for admin dashboard.
    /// </summary>
    Task<IEnumerable<CourseDto>> GetRecentCoursesAsync(int count = 5);

    /// <summary>
    /// Gets recent completions for admin dashboard.
    /// </summary>
    Task<IEnumerable<CompletionDto>> GetRecentCompletionsAsync(int count = 5);

    /// <summary>
    /// Gets course assignments for the specified user.
    /// </summary>
    Task<IEnumerable<AssignmentDto>> GetUserAssignmentsAsync(string userId);

    /// <summary>
    /// Gets detailed learner progress for a specific assignment.
    /// </summary>
    Task<ProgressDto?> GetProgressAsync(Guid assignmentId);

    /// <summary>
    /// Gets all certificates issued to the specified user.
    /// </summary>
    Task<IEnumerable<CertificateDto>> GetUserCertificatesAsync(string userId);

    /// <summary>
    /// Updates progress for an assignment.
    /// </summary>
    Task<bool> UpdateProgressAsync(Guid assignmentId, decimal percentComplete, string? currentModule = null);
}

public class LMSService : ILMSService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseCompletionRepository _courseCompletionRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly ILearnerProgressRepository _learnerProgressRepository;
    private readonly ILogger<LMSService> _logger;

    public LMSService(
        ICourseRepository courseRepository,
        IAssignmentRepository assignmentRepository,
        ICourseCompletionRepository courseCompletionRepository,
        ICertificateRepository certificateRepository,
        ILearnerProgressRepository learnerProgressRepository,
        ILogger<LMSService> logger)
    {
        _courseRepository = courseRepository;
        _assignmentRepository = assignmentRepository;
        _courseCompletionRepository = courseCompletionRepository;
        _certificateRepository = certificateRepository;
        _learnerProgressRepository = learnerProgressRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CourseDto>> GetPublishedCoursesAsync()
    {
        try
        {
            var courses = await _courseRepository.GetPublishedCoursesAsync();
            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                DifficultyLevel = c.Difficulty.ToString(),
                EstimatedDuration = TimeSpan.FromMinutes(c.EstimatedDurationMinutes),
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching published courses from repository");
            return Enumerable.Empty<CourseDto>();
        }
    }

    public async Task<IEnumerable<CourseDto>> GetMyCoursesAsync(string userId)
    {
        try
        {
            // Get assigned courses for the user
            var assignments = await _assignmentRepository.GetByEmployeeIdAsync(userId);
            var courseIds = assignments.Select(a => a.CourseId).Distinct();

            var courseDtos = new List<CourseDto>();
            foreach (var courseId in courseIds)
            {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course != null)
                {
                    courseDtos.Add(new CourseDto
                    {
                        Id = course.Id,
                        Title = course.Title,
                        Description = course.Description,
                        DifficultyLevel = course.Difficulty.ToString(),
                        EstimatedDuration = TimeSpan.FromMinutes(course.EstimatedDurationMinutes),
                        Status = course.Status.ToString(),
                        CreatedAt = course.CreatedAt,
                        UpdatedAt = course.UpdatedAt
                    });
                }
            }

            return courseDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user courses from repository");
            return Enumerable.Empty<CourseDto>();
        }
    }

    public async Task<IEnumerable<CompletionDto>> GetMyCompletionsAsync(string userId)
    {
        try
        {
            var completions = await _courseCompletionRepository.GetByEmployeeIdAsync(userId);
            return completions.Select(c => new CompletionDto
            {
                Id = c.Id,
                CourseId = c.CourseId,
                CourseTitle = c.Course?.Title ?? string.Empty,
                UserId = c.EmployeeId,
                UserName = userId,
                CompletedAt = c.CompletedAt,
                Score = (int)c.FinalScore,
                CertificateIssued = c.CertificateIssued
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user completions from repository");
            return Enumerable.Empty<CompletionDto>();
        }
    }

    public async Task<AdminStatsDto> GetAdminStatsAsync()
    {
        try
        {
            var courses = await _courseRepository.GetAllAsync();

            // Get all completions to calculate stats
            var allCompletions = new List<CourseCompletion>();
            foreach (var course in courses)
            {
                var courseCompletions = await _courseCompletionRepository.GetByCourseIdAsync(course.Id);
                allCompletions.AddRange(courseCompletions);
            }

            // Get total assignments count (rough estimate from completions + ongoing progress)
            var totalAssignments = allCompletions.Count;
            var completedCount = allCompletions.Count(c => c.Passed);
            var completionRate = totalAssignments > 0 ? (double)completedCount / totalAssignments * 100 : 0;

            return new AdminStatsDto
            {
                TotalCourses = courses.Count(),
                ActiveAssignments = totalAssignments - completedCount,
                Completions = completedCount,
                CompletionRate = completionRate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching admin stats from repository");
            return new AdminStatsDto();
        }
    }

    public async Task<IEnumerable<CourseDto>> GetRecentCoursesAsync(int count = 5)
    {
        try
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    DifficultyLevel = c.Difficulty.ToString(),
                    EstimatedDuration = TimeSpan.FromMinutes(c.EstimatedDurationMinutes),
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent courses from repository");
            return Enumerable.Empty<CourseDto>();
        }
    }

    public async Task<IEnumerable<CompletionDto>> GetRecentCompletionsAsync(int count = 5)
    {
        try
        {
            var courses = await _courseRepository.GetAllAsync();
            var allCompletions = new List<CourseCompletion>();

            foreach (var course in courses)
            {
                var courseCompletions = await _courseCompletionRepository.GetByCourseIdAsync(course.Id);
                allCompletions.AddRange(courseCompletions);
            }

            return allCompletions
                .OrderByDescending(c => c.CompletedAt)
                .Take(count)
                .Select(c => new CompletionDto
                {
                    Id = c.Id,
                    CourseId = c.CourseId,
                    CourseTitle = c.Course?.Title ?? string.Empty,
                    UserId = c.EmployeeId,
                    UserName = c.EmployeeId,
                    CompletedAt = c.CompletedAt,
                    Score = (int)c.FinalScore,
                    CertificateIssued = c.CertificateIssued
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent completions from repository");
            return Enumerable.Empty<CompletionDto>();
        }
    }

    public async Task<IEnumerable<AssignmentDto>> GetUserAssignmentsAsync(string userId)
    {
        try
        {
            var assignments = await _assignmentRepository.GetByEmployeeIdAsync(userId);
            return assignments.Select(a => new AssignmentDto
            {
                Id = a.Id,
                CourseId = a.CourseId,
                CourseTitle = a.Course?.Title ?? string.Empty,
                UserId = a.EmployeeId,
                AssignedDate = a.AssignedDate,
                DueDate = a.DueDate,
                Status = a.Status.ToString(),
                IsMandatory = false, // Assignment entity doesn't have this field
                PercentComplete = 0 // TODO: Calculate from progress
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user assignments from repository");
            return Enumerable.Empty<AssignmentDto>();
        }
    }

    public async Task<ProgressDto?> GetProgressAsync(Guid assignmentId)
    {
        try
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null) return null;

            var progress = await _learnerProgressRepository.GetByEmployeeAndCourseAsync(assignment.EmployeeId, assignment.CourseId);
            if (progress == null) return null;

            return new ProgressDto
            {
                Id = progress.Id,
                AssignmentId = assignmentId,
                PercentComplete = progress.ProgressPercentage,
                StartedDate = progress.CreatedAt,
                CompletionDate = null,
                CurrentModule = progress.CurrentModuleId?.ToString(),
                LastAccessedDate = progress.LastAccessedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching progress from repository");
            return null;
        }
    }

    public async Task<IEnumerable<CertificateDto>> GetUserCertificatesAsync(string userId)
    {
        try
        {
            var certificates = await _certificateRepository.GetByEmployeeAsync(userId);
            return certificates.Select(c => new CertificateDto
            {
                Id = c.Id,
                CertificateNumber = c.CertificateNumber,
                CourseTitle = c.Course?.Title ?? string.Empty,
                UserId = c.UserId,
                IssuedDate = c.IssuedDate,
                ExpirationDate = c.ExpirationDate,
                Score = c.Score ?? 0,
                Status = c.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user certificates from repository");
            return Enumerable.Empty<CertificateDto>();
        }
    }

    public async Task<bool> UpdateProgressAsync(Guid assignmentId, decimal percentComplete, string? currentModule = null)
    {
        try
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null) return false;

            var progress = await _learnerProgressRepository.GetByEmployeeAndCourseAsync(assignment.EmployeeId, assignment.CourseId);
            if (progress != null)
            {
                progress.ProgressPercentage = (int)percentComplete;
                progress.LastAccessedAt = DateTime.UtcNow;
                if (currentModule != null && Guid.TryParse(currentModule, out var moduleId))
                {
                    progress.CurrentModuleId = moduleId;
                }
                await _learnerProgressRepository.UpdateAsync(progress);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating progress in repository");
            return false;
        }
    }
}

// DTOs for LMS data transfer
public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public TimeSpan EstimatedDuration { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CompletionDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public int Score { get; set; }
    public bool CertificateIssued { get; set; }
}

public class AdminStatsDto
{
    public int TotalCourses { get; set; }
    public int ActiveAssignments { get; set; }
    public int Completions { get; set; }
    public double CompletionRate { get; set; }
}

public class AssignmentDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public decimal PercentComplete { get; set; }
}

public class ProgressDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public decimal PercentComplete { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal Score { get; set; }
    public int TimeSpentMinutes { get; set; }
    public string? CurrentModule { get; set; }
    public DateTime? LastAccessedDate { get; set; }
}

public class CertificateDto
{
    public Guid Id { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CertificateUrl { get; set; }
}
