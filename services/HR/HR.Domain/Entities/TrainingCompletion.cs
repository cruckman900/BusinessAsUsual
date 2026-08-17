namespace HR.Domain.Entities;

/// <summary>
/// Represents a training course completion record for an employee.
/// Populated from LMS CourseCompletedEvent integration events.
/// </summary>
public class TrainingCompletion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key to Employee
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to Employee
    /// </summary>
    public Employee? Employee { get; set; }

    /// <summary>
    /// Course ID from LMS system
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Course title/name
    /// </summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    /// Date the course was completed
    /// </summary>
    public DateTime CompletionDate { get; set; }

    /// <summary>
    /// Final score achieved (0-100)
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// Certificate number if issued
    /// </summary>
    public string? CertificateNumber { get; set; }

    /// <summary>
    /// Total time spent on the course in minutes
    /// </summary>
    public int TimeSpentMinutes { get; set; }

    /// <summary>
    /// Source integration event ID for idempotency
    /// </summary>
    public Guid SourceEventId { get; set; }

    /// <summary>
    /// When this record was created in HR system
    /// </summary>
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
