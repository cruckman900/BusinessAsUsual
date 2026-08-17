namespace LMS.Domain.Entities;

/// <summary>
/// Represents a course assignment to an employee
/// </summary>
public class Assignment : BaseEntity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string EmployeeId { get; set; } = string.Empty; // Reference to HR employee
    public string? AssignedBy { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.NotStarted;

    // Completion tracking
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? CompletionId { get; set; }
    public CourseCompletion? Completion { get; set; }
}

public enum AssignmentStatus
{
    NotStarted,
    InProgress,
    Completed,
    Overdue
}
