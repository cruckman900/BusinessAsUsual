namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by LMS when an employee starts a training course.
/// HR can consume this to track employee engagement and learning activity.
/// </summary>
public sealed class TrainingStartedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "lms.training.started";

    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = string.Empty;
    public string EmployeeId { get; init; } = string.Empty;
    public DateTime StartedDate { get; init; }
    public bool IsAssignedTraining { get; init; }
    public Guid? AssignmentId { get; init; }
}
