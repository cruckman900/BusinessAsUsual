using LMS.Application.Common;
using LMS.Contracts.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Learning.Queries;

public class GetProgressQuery : IQuery<Result<LearnerProgressDto>>
{
    public string EmployeeId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
}

public class LearnerProgressDto
{
    public Guid Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public Guid? CurrentModuleId { get; set; }
    public Guid? CurrentLessonId { get; set; }
    public List<Guid> CompletedModules { get; set; } = new();
    public List<Guid> CompletedLessons { get; set; } = new();
    public List<Guid> CompletedQuizzes { get; set; } = new();
    public int ProgressPercentage { get; set; }
    public DateTime LastAccessedAt { get; set; }
}

public class GetProgressQueryHandler : IQueryHandler<GetProgressQuery, Result<LearnerProgressDto>>
{
    private readonly ILearnerProgressRepository _progressRepository;
    private readonly ILogger<GetProgressQueryHandler> _logger;

    public GetProgressQueryHandler(
        ILearnerProgressRepository progressRepository,
        ILogger<GetProgressQueryHandler> logger)
    {
        _progressRepository = progressRepository;
        _logger = logger;
    }

    public async Task<Result<LearnerProgressDto>> HandleAsync(GetProgressQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var progress = await _progressRepository.GetByEmployeeAndCourseAsync(query.EmployeeId, query.CourseId, cancellationToken);
            if (progress == null)
                return Result<LearnerProgressDto>.Fail("Progress not found");

            var dto = new LearnerProgressDto
            {
                Id = progress.Id,
                EmployeeId = progress.EmployeeId,
                CourseId = progress.CourseId,
                CurrentModuleId = progress.CurrentModuleId,
                CurrentLessonId = progress.CurrentLessonId,
                CompletedModules = progress.CompletedModules,
                CompletedLessons = progress.CompletedLessons,
                CompletedQuizzes = progress.CompletedQuizzes,
                ProgressPercentage = progress.ProgressPercentage,
                LastAccessedAt = progress.LastAccessedAt
            };

            return Result<LearnerProgressDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting progress for employee {EmployeeId} in course {CourseId}", query.EmployeeId, query.CourseId);
            return Result<LearnerProgressDto>.Fail($"Error retrieving progress: {ex.Message}");
        }
    }
}
