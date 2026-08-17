namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by LMS when an employee completes a training course.
/// HR consumes this to update employee training records and track compliance.
/// </summary>
public sealed class TrainingCompletedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "lms.training.completed";

    public Guid CompletionId { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = string.Empty;
    public string EmployeeId { get; init; } = string.Empty;
    public DateTime CompletedDate { get; init; }
    public decimal FinalScore { get; init; }
    public bool Passed { get; init; }
    public int DurationMinutes { get; init; }
}
