namespace LMS.Domain.Entities;

/// <summary>
/// Represents the assignment of a course to a learner
/// </summary>
public class CourseAssignment : BaseEntity
{
    /// <summary>
    /// The user/employee ID who is assigned the course
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The course that is assigned
    /// </summary>
    public Guid CourseId { get; set; }
    public Course? Course { get; set; }

    /// <summary>
    /// Who assigned this course (admin user ID)
    /// </summary>
    public string AssignedBy { get; set; } = string.Empty;

    /// <summary>
    /// When the assignment was created
    /// </summary>
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional due date for completion
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Assignment status: Assigned, InProgress, Completed, Overdue, Cancelled
    /// </summary>
    public CourseAssignmentStatus Status { get; set; } = CourseAssignmentStatus.Assigned;

    /// <summary>
    /// Optional notes from the assigner
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this assignment is mandatory
    /// </summary>
    public bool IsMandatory { get; set; } = false;
}

public enum CourseAssignmentStatus
{
    Assigned,
    InProgress,
    Completed,
    Overdue,
    Cancelled
}
