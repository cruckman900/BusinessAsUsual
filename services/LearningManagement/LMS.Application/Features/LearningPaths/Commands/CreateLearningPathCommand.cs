using LMS.Application.Common;
using LMS.Domain.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.LearningPaths.Commands;

public class CreateLearningPathCommand : ICommand<Result<Guid>>
{
    public CreateLearningPathRequest Request { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}

public class CreateLearningPathCommandHandler : ICommandHandler<CreateLearningPathCommand, Result<Guid>>
{
    private readonly ILearningPathRepository _pathRepository;
    private readonly ICourseRepository _courseRepository;

    public CreateLearningPathCommandHandler(
        ILearningPathRepository pathRepository,
        ICourseRepository courseRepository)
    {
        _pathRepository = pathRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Result<Guid>> HandleAsync(CreateLearningPathCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Validate courses exist
            foreach (var courseInput in command.Request.Courses)
            {
                var course = await _courseRepository.GetByIdAsync(courseInput.CourseId);
                if (course == null)
                {
                    return Result<Guid>.Fail($"Course with ID {courseInput.CourseId} not found");
                }
            }

            // Create learning path
            var path = new LearningPath
            {
                Title = command.Request.Title,
                Description = command.Request.Description,
                Category = command.Request.Category,
                Difficulty = Enum.TryParse<CourseDifficulty>(command.Request.Difficulty, out var diff) ? diff : CourseDifficulty.Beginner,
                EstimatedHours = command.Request.EstimatedHours,
                IsPublished = false,
                CreatedBy = command.CreatedBy
            };

            // Add courses to path
            foreach (var courseInput in command.Request.Courses.OrderBy(c => c.OrderIndex))
            {
                path.Courses.Add(new LearningPathCourse
                {
                    CourseId = courseInput.CourseId,
                    OrderIndex = courseInput.OrderIndex,
                    IsRequired = courseInput.IsRequired
                });
            }

            var createdPath = await _pathRepository.AddAsync(path, cancellationToken);
            return Result<Guid>.Ok(createdPath.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail($"Failed to create learning path: {ex.Message}");
        }
    }
}
