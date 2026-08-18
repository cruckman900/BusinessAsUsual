using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class PublishCourseCommand : ICommand<Result>
{
    public Guid CourseId { get; set; }
    public string? PublishedBy { get; set; }
}

public class PublishCourseCommandHandler : ICommandHandler<PublishCourseCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<PublishCourseCommandHandler> _logger;

    public PublishCourseCommandHandler(
        ICourseRepository courseRepository,
        IEventBus eventBus,
        ILogger<PublishCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(PublishCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetWithModulesAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result.Fail("Course not found");

            // Validate course has content
            if (!course.Modules.Any())
                return Result.Fail("Cannot publish course without modules");

            if (course.Status == CourseStatus.Published)
                return Result.Fail("Course is already published");

            // Publish
            course.Status = CourseStatus.Published;
            course.PublishedDate = DateTime.UtcNow;
            course.PublishedBy = command.PublishedBy;
            course.LastModifiedDate = DateTime.UtcNow;
            course.LastModifiedBy = command.PublishedBy;
            course.UpdatedAt = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Course published: {CourseId} - {Title}", course.Id, course.Title);

            // TODO: Publish event if needed for notifications

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing course: {CourseId}", command.CourseId);
            return Result.Fail($"Error publishing course: {ex.Message}");
        }
    }
}
