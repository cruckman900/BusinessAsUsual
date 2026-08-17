using LMS.Application.Common;
using LMS.Contracts.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Queries;

public class GetCourseQuery : IQuery<Result<CourseDto>>
{
    public Guid CourseId { get; set; }
}

public class GetCourseQueryHandler : IQueryHandler<GetCourseQuery, Result<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<GetCourseQueryHandler> _logger;

    public GetCourseQueryHandler(
        ICourseRepository courseRepository,
        ILogger<GetCourseQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<CourseDto>> HandleAsync(GetCourseQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(query.CourseId, cancellationToken);
            if (course == null)
                return Result<CourseDto>.Fail("Course not found");

            var dto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                ThumbnailUrl = course.ThumbnailUrl,
                Status = course.Status.ToString(),
                Difficulty = course.Difficulty.ToString(),
                EstimatedDurationMinutes = course.EstimatedDurationMinutes,
                Category = course.Category,
                Tags = course.Tags,
                PublishedDate = course.PublishedDate,
                RequiresAssessment = course.RequiresAssessment,
                PassingScore = course.PassingScore,
                IssuesCertificate = course.IssuesCertificate
            };

            return Result<CourseDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course: {CourseId}", query.CourseId);
            return Result<CourseDto>.Fail($"Error retrieving course: {ex.Message}");
        }
    }
}
