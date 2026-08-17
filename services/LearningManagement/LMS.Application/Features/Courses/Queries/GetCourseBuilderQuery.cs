using LMS.Application.Common;
using LMS.Contracts.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Queries;

public class GetCourseBuilderQuery : IQuery<Result<CourseBuilderDto>>
{
    public Guid CourseId { get; set; }
}

public class CourseBuilderDto
{
    public CourseDto Course { get; set; } = null!;
    public List<ModuleDto> Modules { get; set; } = new();
}

public class GetCourseBuilderQueryHandler : IQueryHandler<GetCourseBuilderQuery, Result<CourseBuilderDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<GetCourseBuilderQueryHandler> _logger;

    public GetCourseBuilderQueryHandler(
        ICourseRepository courseRepository,
        ILogger<GetCourseBuilderQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<CourseBuilderDto>> HandleAsync(GetCourseBuilderQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetWithFullStructureAsync(query.CourseId, cancellationToken);
            if (course == null)
                return Result<CourseBuilderDto>.Fail("Course not found");

            var dto = new CourseBuilderDto
            {
                Course = new CourseDto
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
                    IssuesCertificate = course.IssuesCertificate,
                    ModuleCount = course.Modules.Count,
                    LessonCount = course.Modules.SelectMany(m => m.Lessons).Count()
                },
                Modules = course.Modules.OrderBy(m => m.OrderIndex).Select(m => new ModuleDto
                {
                    Id = m.Id,
                    CourseId = m.CourseId,
                    Title = m.Title,
                    Description = m.Description,
                    OrderIndex = m.OrderIndex,
                    Lessons = m.Lessons.OrderBy(l => l.OrderIndex).Select(l => new LessonDto
                    {
                        Id = l.Id,
                        ModuleId = l.ModuleId,
                        Title = l.Title,
                        Description = l.Description,
                        OrderIndex = l.OrderIndex,
                        EstimatedDurationMinutes = l.EstimatedDurationMinutes,
                        ContentBlocks = l.ContentBlocks.OrderBy(cb => cb.OrderIndex).Select(cb => new ContentBlockDto
                        {
                            Id = cb.Id,
                            LessonId = cb.LessonId,
                            BlockType = cb.BlockType.ToString(),
                            OrderIndex = cb.OrderIndex,
                            Content = System.Text.Json.JsonSerializer.Deserialize<object>(cb.JsonContent),
                            QuizId = cb.QuizId
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return Result<CourseBuilderDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course builder data: {CourseId}", query.CourseId);
            return Result<CourseBuilderDto>.Fail($"Error retrieving course: {ex.Message}");
        }
    }
}
