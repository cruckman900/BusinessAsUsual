using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetByEmployeeIdAsync(string employeeId, bool unreadOnly = false, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<Notification> UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string employeeId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
