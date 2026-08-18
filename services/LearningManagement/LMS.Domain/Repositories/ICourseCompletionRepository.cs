using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface ICourseCompletionRepository
{
    Task<CourseCompletion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CourseCompletion?> GetByEmployeeAndCourseAsync(string employeeId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CourseCompletion>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CourseCompletion>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CourseCompletion>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CourseCompletion> AddAsync(CourseCompletion completion, CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseCompletion completion, CancellationToken cancellationToken = default);
}
