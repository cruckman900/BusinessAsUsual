using LMS.Application.Common;
using LMS.Domain.DTOs;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.LearningPaths.Queries;

public class GetLearningPathsQuery : IQuery<Result<List<LearningPathDto>>>
{
    public bool PublishedOnly { get; set; } = true;
}

public class GetLearningPathsQueryHandler : IQueryHandler<GetLearningPathsQuery, Result<List<LearningPathDto>>>
{
    private readonly ILearningPathRepository _pathRepository;

    public GetLearningPathsQueryHandler(ILearningPathRepository pathRepository)
    {
        _pathRepository = pathRepository;
    }

    public async Task<Result<List<LearningPathDto>>> HandleAsync(GetLearningPathsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var paths = query.PublishedOnly
                ? await _pathRepository.GetPublishedAsync(cancellationToken)
                : await _pathRepository.GetAllAsync(cancellationToken);

            var dtos = paths.Select(p => new LearningPathDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Category = p.Category,
                Difficulty = p.Difficulty.ToString(),
                EstimatedHours = p.EstimatedHours,
                IsPublished = p.IsPublished,
                CourseCount = p.Courses.Count,
                Courses = p.Courses.OrderBy(c => c.OrderIndex).Select(c => new LearningPathCourseDto
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.Course?.Title ?? "Unknown",
                    OrderIndex = c.OrderIndex,
                    IsRequired = c.IsRequired,
                    IsCompleted = false,
                    IsLocked = false
                }).ToList()
            }).ToList();

            return Result<List<LearningPathDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<LearningPathDto>>.Fail($"Failed to retrieve learning paths: {ex.Message}");
        }
    }
}
