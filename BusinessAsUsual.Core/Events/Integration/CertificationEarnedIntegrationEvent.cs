namespace BusinessAsUsual.Core.Events.Integration;

/// <summary>
/// Raised by LMS when a certificate is issued to an employee for course completion.
/// HR consumes this to update employee certification records.
/// </summary>
public sealed class CertificationEarnedIntegrationEvent : IntegrationEvent
{
    public override string EventType => "lms.certification.earned";

    public Guid CertificationId { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = string.Empty;
    public string CertificateName { get; init; } = string.Empty;
    public string EmployeeId { get; init; } = string.Empty;
    public DateTime IssuedDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string? CertificateUrl { get; init; }
    public decimal FinalScore { get; init; }
}
