using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Notifications.Queries;

public class GetMyNotificationsQuery : IQuery<List<Notification>>
{
    public string EmployeeId { get; set; } = string.Empty;
    public bool UnreadOnly { get; set; } = false;
}

public class GetMyNotificationsQueryHandler : IQueryHandler<GetMyNotificationsQuery, List<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetMyNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<Notification>> HandleAsync(
        GetMyNotificationsQuery query, 
        CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetByEmployeeIdAsync(
            query.EmployeeId, 
            query.UnreadOnly, 
            cancellationToken);
    }
}
