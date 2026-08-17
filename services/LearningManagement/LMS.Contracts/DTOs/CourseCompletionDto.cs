namespace LMS.Contracts.DTOs;

public class CourseCompletionDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public decimal FinalScore { get; set; }
    public bool Passed { get; set; }
    public bool CertificateIssued { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTime? CertificateExpiryDate { get; set; }
}
