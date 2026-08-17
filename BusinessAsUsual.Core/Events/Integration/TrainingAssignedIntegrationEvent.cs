namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by LMS when a training course is assigned to an employee.
/// HR can consume this to track training requirements and due dates.
/// </summary>
public sealed class TrainingAssignedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "lms.training.assigned";

    public Guid AssignmentId { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = string.Empty;
    public string EmployeeId { get; init; } = string.Empty;
    public string? AssignedBy { get; init; }
    public DateTime AssignedDate { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsMandatory { get; init; }
}
