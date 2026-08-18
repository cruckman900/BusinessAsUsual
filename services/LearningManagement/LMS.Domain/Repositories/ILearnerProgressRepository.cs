using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface ILearnerProgressRepository
{
    Task<LearnerProgress?> GetByEmployeeAndCourseAsync(string employeeId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LearnerProgress>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LearnerProgress>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LearnerProgress> AddAsync(LearnerProgress progress, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearnerProgress progress, CancellationToken cancellationToken = default);
}
