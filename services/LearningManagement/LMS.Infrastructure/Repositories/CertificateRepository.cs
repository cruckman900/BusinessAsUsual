using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private readonly LMSDbContext _context;

    public CertificateRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<Certificate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c => c.CertificateNumber == certificateNumber, cancellationToken);
    }

    public async Task<List<Certificate>> GetByEmployeeAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Course)
            .Where(c => c.UserId == employeeId)
            .OrderByDescending(c => c.IssuedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Certificate>> GetByEmployeeAndCourseAsync(string employeeId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Course)
            .Where(c => c.UserId == employeeId && c.CourseId == courseId)
            .OrderByDescending(c => c.IssuedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Certificate>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Certificates
            .Include(c => c.Course)
            .Where(c => c.CourseId == courseId)
            .OrderByDescending(c => c.IssuedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Certificate> AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync(cancellationToken);
        return certificate;
    }

    public async Task<Certificate> UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        _context.Certificates.Update(certificate);
        await _context.SaveChangesAsync(cancellationToken);
        return certificate;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var certificate = await GetByIdAsync(id, cancellationToken);
        if (certificate != null)
        {
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
