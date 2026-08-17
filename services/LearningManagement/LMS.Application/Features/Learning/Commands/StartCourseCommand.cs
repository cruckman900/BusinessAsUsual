using LMS.Application.Common;
using LMS.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Learning.Commands;

public class StartCourseCommand : ICommand<Result>
{
    public Guid CourseId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public Guid? AssignmentId { get; set; }
}

public class StartCourseCommandHandler : ICommandHandler<StartCourseCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILearnerProgressRepository _progressRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<StartCourseCommandHandler> _logger;

    public StartCourseCommandHandler(
        ICourseRepository courseRepository,
        ILearnerProgressRepository progressRepository,
        IAssignmentRepository assignmentRepository,
        IEventBus eventBus,
        ILogger<StartCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _progressRepository = progressRepository;
        _assignmentRepository = assignmentRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(StartCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetWithModulesAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result.Fail("Course not found");

            var progress = await _progressRepository.GetByEmployeeAndCourseAsync(command.EmployeeId, command.CourseId, cancellationToken);
            if (progress == null)
                return Result.Fail("Not enrolled in this course");

            // Set current module/lesson to first
            var firstModule = course.Modules.OrderBy(m => m.OrderIndex).FirstOrDefault();
            if (firstModule != null)
            {
                var firstLesson = firstModule.Lessons.OrderBy(l => l.OrderIndex).FirstOrDefault();
                progress.CurrentModuleId = firstModule.Id;
                progress.CurrentLessonId = firstLesson?.Id;
            }

            progress.LastAccessedAt = DateTime.UtcNow;
            await _progressRepository.UpdateAsync(progress, cancellationToken);

            // Update assignment if provided
            if (command.AssignmentId.HasValue)
            {
                var assignment = await _assignmentRepository.GetByIdAsync(command.AssignmentId.Value, cancellationToken);
                if (assignment != null)
                {
                    assignment.Status = Domain.Entities.AssignmentStatus.InProgress;
                    assignment.StartedAt = DateTime.UtcNow;
                    await _assignmentRepository.UpdateAsync(assignment, cancellationToken);
                }
            }

            // Publish event
            var integrationEvent = new TrainingStartedIntegrationEvent
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                EmployeeId = command.EmployeeId,
                StartedDate = DateTime.UtcNow,
                IsAssignedTraining = command.AssignmentId.HasValue,
                AssignmentId = command.AssignmentId
            };
            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            _logger.LogInformation("Employee {EmployeeId} started course {CourseId}", command.EmployeeId, command.CourseId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting course: {CourseId}", command.CourseId);
            return Result.Fail($"Error starting course: {ex.Message}");
        }
    }
}
