using LMS.Application.Common;
using LMS.Contracts.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Learning.Queries;

public class GetMyCoursesQuery : IQuery<Result<List<CourseDto>>>
{
    public string EmployeeId { get; set; } = string.Empty;
    public bool CompletedOnly { get; set; } = false;
}

public class GetMyCoursesQueryHandler : IQueryHandler<GetMyCoursesQuery, Result<List<CourseDto>>>
{
    private readonly ILearnerProgressRepository _progressRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseCompletionRepository _completionRepository;
    private readonly ILogger<GetMyCoursesQueryHandler> _logger;

    public GetMyCoursesQueryHandler(
        ILearnerProgressRepository progressRepository,
        ICourseRepository courseRepository,
        ICourseCompletionRepository completionRepository,
        ILogger<GetMyCoursesQueryHandler> logger)
    {
        _progressRepository = progressRepository;
        _courseRepository = courseRepository;
        _completionRepository = completionRepository;
        _logger = logger;
    }

    public async Task<Result<List<CourseDto>>> HandleAsync(GetMyCoursesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            List<Guid> courseIds;

            if (query.CompletedOnly)
            {
                var completions = await _completionRepository.GetByEmployeeIdAsync(query.EmployeeId, cancellationToken);
                courseIds = completions.Select(c => c.CourseId).ToList();
            }
            else
            {
                var progresses = await _progressRepository.GetByEmployeeIdAsync(query.EmployeeId, cancellationToken);
                courseIds = progresses.Select(p => p.CourseId).ToList();
            }

            var courses = await _courseRepository.GetAllAsync(cancellationToken);
            var myCourses = courses.Where(c => courseIds.Contains(c.Id)).ToList();

            var dtos = myCourses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ThumbnailUrl = c.ThumbnailUrl,
                Status = c.Status.ToString(),
                Difficulty = c.Difficulty.ToString(),
                EstimatedDurationMinutes = c.EstimatedDurationMinutes,
                Category = c.Category,
                Tags = c.Tags,
                PublishedDate = c.PublishedDate,
                RequiresAssessment = c.RequiresAssessment,
                PassingScore = c.PassingScore,
                IssuesCertificate = c.IssuesCertificate,
                ModuleCount = c.Modules.Count,
                LessonCount = c.Modules.SelectMany(m => m.Lessons).Count()
            }).ToList();

            return Result<List<CourseDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses for employee: {EmployeeId}", query.EmployeeId);
            return Result<List<CourseDto>>.Fail($"Error retrieving courses: {ex.Message}");
        }
    }
}
