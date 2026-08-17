using LMS.Application.Common;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class DeleteCourseCommand : ICommand<Result>
{
    public Guid CourseId { get; set; }
}

public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;

    public DeleteCourseCommandHandler(
        ICourseRepository courseRepository,
        ILogger<DeleteCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result.Fail("Course not found");

            // Soft delete
            course.IsDeleted = true;
            course.UpdatedAt = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Course deleted: {CourseId}", command.CourseId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course: {CourseId}", command.CourseId);
            return Result.Fail($"Error deleting course: {ex.Message}");
        }
    }
}
