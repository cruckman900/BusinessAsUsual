using LMS.Application.Common;
using LMS.Contracts.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Queries;

public class ListCoursesQuery : IQuery<Result<List<CourseDto>>>
{
    public bool PublishedOnly { get; set; } = false;
    public string? AuthorId { get; set; }
}

public class ListCoursesQueryHandler : IQueryHandler<ListCoursesQuery, Result<List<CourseDto>>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<ListCoursesQueryHandler> _logger;

    public ListCoursesQueryHandler(
        ICourseRepository courseRepository,
        ILogger<ListCoursesQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<List<CourseDto>>> HandleAsync(ListCoursesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var courses = query.PublishedOnly 
                ? await _courseRepository.GetPublishedCoursesAsync(cancellationToken)
                : !string.IsNullOrEmpty(query.AuthorId)
                    ? await _courseRepository.GetByAuthorAsync(query.AuthorId, cancellationToken)
                    : await _courseRepository.GetAllAsync(cancellationToken);

            var dtos = courses.Select(c => new CourseDto
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
            _logger.LogError(ex, "Error listing courses");
            return Result<List<CourseDto>>.Fail($"Error retrieving courses: {ex.Message}");
        }
    }
}
