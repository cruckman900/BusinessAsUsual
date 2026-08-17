using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Course?> GetWithModulesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Course?> GetWithFullStructureAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetByAuthorAsync(string authorId, CancellationToken cancellationToken = default);
    Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course course, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
