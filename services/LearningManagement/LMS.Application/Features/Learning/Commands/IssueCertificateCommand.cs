using LMS.Application.Common;
using LMS.Application.Features.Notifications.Commands;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Learning.Commands;

public class IssueCertificateCommand : ICommand<Result<Certificate>>
{
    public Guid CourseId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public decimal? FinalScore { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class IssueCertificateCommandHandler : ICommandHandler<IssueCertificateCommand, Result<Certificate>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseCompletionRepository _completionRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly ICommandHandler<SendNotificationCommand, Result<Notification>> _notificationHandler;

    public IssueCertificateCommandHandler(
        ICourseRepository courseRepository,
        ICourseCompletionRepository completionRepository,
        ICertificateRepository certificateRepository,
        ICommandHandler<SendNotificationCommand, Result<Notification>> notificationHandler)
    {
        _courseRepository = courseRepository;
        _completionRepository = completionRepository;
        _certificateRepository = certificateRepository;
        _notificationHandler = notificationHandler;
    }

    public async Task<Result<Certificate>> HandleAsync(
        IssueCertificateCommand command, 
        CancellationToken cancellationToken = default)
    {
        // Get the course
        var course = await _courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
        if (course == null)
        {
            return Result<Certificate>.Fail("Course not found");
        }

        // Get completion record
        var completion = await _completionRepository.GetByEmployeeAndCourseAsync(
            command.EmployeeId, 
            command.CourseId, 
            cancellationToken);

        if (completion == null || !completion.Passed)
        {
            return Result<Certificate>.Fail("Course must be completed and passed before certificate can be issued");
        }

        // Check if certificate already exists
        var existingCertificates = await _certificateRepository.GetByEmployeeAndCourseAsync(
            command.EmployeeId, 
            command.CourseId, 
            cancellationToken);

        var activeCert = existingCertificates.FirstOrDefault(c => c.Status == CertificateStatus.Active);
        if (activeCert != null)
        {
            return Result<Certificate>.Ok(activeCert); // Already has active certificate
        }

        // Generate certificate number (format: BAU-LMS-YYYYMMDD-XXXXX)
        var certNumber = $"BAU-LMS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}";

        // Create certificate
        var certificate = new Certificate
        {
            Id = Guid.NewGuid(),
            CertificateNumber = certNumber,
            UserId = command.EmployeeId,
            CourseId = command.CourseId,
            Course = course,
            IssuedDate = DateTime.UtcNow,
            ExpirationDate = command.ExpiryDate ?? (course.CertificateValidityDays.HasValue 
                ? DateTime.UtcNow.AddDays(course.CertificateValidityDays.Value) 
                : null),
            Score = command.FinalScore ?? completion.FinalScore,
            Status = CertificateStatus.Active,
            IssuedBy = "Business As Usual LMS",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = command.EmployeeId
        };

        await _certificateRepository.AddAsync(certificate, cancellationToken);

        // Send notification
        var notificationCommand = new SendNotificationCommand
        {
            EmployeeId = command.EmployeeId,
            Type = NotificationType.CertificateIssued,
            Title = "Certificate Issued! 🎓",
            Message = $"Congratulations! You've earned a certificate for completing {course.Title}",
            ActionUrl = $"/lms/my-certificates",
            ActionText = "View Certificate",
            Priority = NotificationPriority.High,
            CourseId = command.CourseId,
            CertificateId = certificate.Id,
            SendEmail = true
        };

        await _notificationHandler.HandleAsync(notificationCommand, cancellationToken);

        // TODO: Generate PDF certificate

        return Result<Certificate>.Ok(certificate);
    }
}
