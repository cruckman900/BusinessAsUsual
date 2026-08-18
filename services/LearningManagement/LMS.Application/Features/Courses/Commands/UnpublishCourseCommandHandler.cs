using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class UnpublishCourseCommandHandler
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<UnpublishCourseCommandHandler> _logger;

    public UnpublishCourseCommandHandler(
        ICourseRepository courseRepository,
        ILogger<UnpublishCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UnpublishCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result.Fail("Course not found");

            if (course.Status == CourseStatus.Draft)
                return Result.Fail("Course is already in draft status");

            course.Status = CourseStatus.Draft;
            course.LastModifiedDate = DateTime.UtcNow;
            course.LastModifiedBy = command.ModifiedBy;
            course.UpdatedAt = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Course unpublished: {CourseId} - {Title}", course.Id, course.Title);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing course: {CourseId}", command.CourseId);
            return Result.Fail($"Error unpublishing course: {ex.Message}");
        }
    }
}
