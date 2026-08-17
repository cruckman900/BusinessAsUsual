namespace LMS.Domain.Entities;

/// <summary>
/// Represents a certificate awarded upon course completion
/// </summary>
public class Certificate : BaseEntity
{
    /// <summary>
    /// Unique certificate number for verification
    /// </summary>
    public string CertificateNumber { get; set; } = string.Empty;

    /// <summary>
    /// The user/employee who earned the certificate
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The course that was completed
    /// </summary>
    public Guid CourseId { get; set; }
    public Course? Course { get; set; }

    /// <summary>
    /// When the course was completed and certificate issued
    /// </summary>
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional expiration date (for certifications requiring renewal)
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Final score achieved
    /// </summary>
    public decimal? Score { get; set; }

    /// <summary>
    /// Certificate status: Active, Expired, Revoked
    /// </summary>
    public CertificateStatus Status { get; set; } = CertificateStatus.Active;

    /// <summary>
    /// Optional URL to certificate image/PDF
    /// </summary>
    public string? CertificateUrl { get; set; }

    /// <summary>
    /// Issuing authority name
    /// </summary>
    public string IssuedBy { get; set; } = "Business As Usual LMS";
}

public enum CertificateStatus
{
    Active,
    Expired,
    Revoked
}
