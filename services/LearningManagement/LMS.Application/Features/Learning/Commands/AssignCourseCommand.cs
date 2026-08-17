using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Learning.Commands;

public class AssignCourseCommand : ICommand<Result<List<Guid>>>
{
    public Guid CourseId { get; set; }
    public List<string> EmployeeIds { get; set; } = new();
    public DateTime? DueDate { get; set; }
    public string? AssignedBy { get; set; }
    public bool IsMandatory { get; set; } = true;
}

public class AssignCourseCommandHandler : ICommandHandler<AssignCourseCommand, Result<List<Guid>>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<AssignCourseCommandHandler> _logger;

    public AssignCourseCommandHandler(
        ICourseRepository courseRepository,
        IAssignmentRepository assignmentRepository,
        IEventBus eventBus,
        ILogger<AssignCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _assignmentRepository = assignmentRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Result<List<Guid>>> HandleAsync(AssignCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result<List<Guid>>.Fail("Course not found");

            if (course.Status != CourseStatus.Published)
                return Result<List<Guid>>.Fail("Cannot assign unpublished course");

            var assignmentIds = new List<Guid>();

            foreach (var employeeId in command.EmployeeIds)
            {
                var assignment = new Assignment
                {
                    CourseId = command.CourseId,
                    EmployeeId = employeeId,
                    AssignedBy = command.AssignedBy,
                    AssignedDate = DateTime.UtcNow,
                    DueDate = command.DueDate,
                    Status = AssignmentStatus.NotStarted,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _assignmentRepository.AddAsync(assignment, cancellationToken);
                assignmentIds.Add(created.Id);

                // Publish TrainingAssignedIntegrationEvent to HR
                var integrationEvent = new TrainingAssignedIntegrationEvent
                {
                    AssignmentId = created.Id,
                    CourseId = course.Id,
                    CourseTitle = course.Title,
                    EmployeeId = employeeId,
                    AssignedBy = command.AssignedBy,
                    AssignedDate = assignment.AssignedDate,
                    DueDate = command.DueDate,
                    IsMandatory = command.IsMandatory
                };
                await _eventBus.PublishAsync(integrationEvent, cancellationToken);

                _logger.LogInformation("Course {CourseId} assigned to employee {EmployeeId}", command.CourseId, employeeId);
            }

            return Result<List<Guid>>.Ok(assignmentIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning course: {CourseId}", command.CourseId);
            return Result<List<Guid>>.Fail($"Error assigning course: {ex.Message}");
        }
    }
}
