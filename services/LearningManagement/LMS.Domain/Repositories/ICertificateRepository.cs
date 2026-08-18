using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface ICertificateRepository
{
    Task<Certificate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default);
    Task<List<Certificate>> GetByEmployeeAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<List<Certificate>> GetByEmployeeAndCourseAsync(string employeeId, Guid courseId, CancellationToken cancellationToken = default);
    Task<List<Certificate>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<List<Certificate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Certificate> AddAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task<Certificate> UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
