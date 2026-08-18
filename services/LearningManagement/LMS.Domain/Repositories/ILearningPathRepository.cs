using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface ILearningPathRepository
{
    Task<LearningPath?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LearningPath?> GetWithCoursesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<LearningPath>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<LearningPath>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<LearningPath> AddAsync(LearningPath path, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearningPath path, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ILearningPathEnrollmentRepository
{
    Task<LearningPathEnrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LearningPathEnrollment?> GetByEmployeeAndPathAsync(string employeeId, Guid pathId, CancellationToken cancellationToken = default);
    Task<List<LearningPathEnrollment>> GetByEmployeeAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<LearningPathEnrollment> AddAsync(LearningPathEnrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearningPathEnrollment enrollment, CancellationToken cancellationToken = default);
}

public interface ICoursePrerequisiteRepository
{
    Task<List<CoursePrerequisite>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CoursePrerequisite> AddAsync(CoursePrerequisite prerequisite, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
