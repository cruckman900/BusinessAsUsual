using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class LearnerProgressRepository : ILearnerProgressRepository
{
    private readonly LMSDbContext _context;

    public LearnerProgressRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<LearnerProgress?> GetByEmployeeAndCourseAsync(string employeeId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.LearnerProgresses
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.CourseId == courseId && !p.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<LearnerProgress>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.LearnerProgresses
            .Include(p => p.Course)
            .Where(p => p.EmployeeId == employeeId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<LearnerProgress> AddAsync(LearnerProgress progress, CancellationToken cancellationToken = default)
    {
        _context.LearnerProgresses.Add(progress);
        await _context.SaveChangesAsync(cancellationToken);
        return progress;
    }

    public async Task UpdateAsync(LearnerProgress progress, CancellationToken cancellationToken = default)
    {
        _context.LearnerProgresses.Update(progress);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
