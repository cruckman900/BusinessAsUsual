using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assignment>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assignment>> GetOverdueAssignmentsAsync(CancellationToken cancellationToken = default);
    Task<Assignment> AddAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
