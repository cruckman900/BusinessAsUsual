using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Notifications.Commands;

public class SendNotificationCommand : ICommand<Result<Notification>>
{
    public string EmployeeId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public Guid? CourseId { get; set; }
    public Guid? AssignmentId { get; set; }
    public Guid? CertificateId { get; set; }
    public Guid? QuizAttemptId { get; set; }
    public bool SendEmail { get; set; } = false;
}

public class SendNotificationCommandHandler : ICommandHandler<SendNotificationCommand, Result<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public SendNotificationCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<Notification>> HandleAsync(
        SendNotificationCommand command, 
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            EmployeeId = command.EmployeeId,
            Type = command.Type,
            Title = command.Title,
            Message = command.Message,
            ActionUrl = command.ActionUrl,
            ActionText = command.ActionText,
            Priority = command.Priority,
            CourseId = command.CourseId,
            AssignmentId = command.AssignmentId,
            CertificateId = command.CertificateId,
            QuizAttemptId = command.QuizAttemptId,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);

        // TODO: Send email if requested
        if (command.SendEmail)
        {
            // Email sending logic will go here
        }

        return Result<Notification>.Ok(notification);
    }
}
