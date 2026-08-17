using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CourseCompletionRepository : ICourseCompletionRepository
{
    private readonly LMSDbContext _context;

    public CourseCompletionRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<CourseCompletion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CourseCompletions
            .Include(c => c.Course)
            .Include(c => c.FinalAssessmentAttempt)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<CourseCompletion?> GetByEmployeeAndCourseAsync(string employeeId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.CourseCompletions
            .Include(c => c.Course)
            .Include(c => c.FinalAssessmentAttempt)
            .FirstOrDefaultAsync(c => c.EmployeeId == employeeId && c.CourseId == courseId && !c.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<CourseCompletion>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.CourseCompletions
            .Include(c => c.Course)
            .Include(c => c.FinalAssessmentAttempt)
            .Where(c => c.EmployeeId == employeeId && !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CourseCompletion>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.CourseCompletions
            .Include(c => c.Course)
            .Include(c => c.FinalAssessmentAttempt)
            .Where(c => c.CourseId == courseId && !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<CourseCompletion> AddAsync(CourseCompletion completion, CancellationToken cancellationToken = default)
    {
        _context.CourseCompletions.Add(completion);
        await _context.SaveChangesAsync(cancellationToken);
        return completion;
    }

    public async Task UpdateAsync(CourseCompletion completion, CancellationToken cancellationToken = default)
    {
        _context.CourseCompletions.Update(completion);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
