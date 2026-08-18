using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class LearningPathRepository : ILearningPathRepository
{
    private readonly LMSDbContext _context;

    public LearningPathRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<LearningPath?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Where(p => !p.IsDeleted && p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LearningPath?> GetWithCoursesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Include(p => p.Courses)
                .ThenInclude(c => c.Course)
            .Where(p => !p.IsDeleted && p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<LearningPath>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Include(p => p.Courses)
                .ThenInclude(c => c.Course)
            .Where(p => !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LearningPath>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LearningPaths
            .Include(p => p.Courses)
                .ThenInclude(c => c.Course)
            .Where(p => !p.IsDeleted && p.IsPublished)
            .ToListAsync(cancellationToken);
    }

    public async Task<LearningPath> AddAsync(LearningPath path, CancellationToken cancellationToken = default)
    {
        _context.LearningPaths.Add(path);
        await _context.SaveChangesAsync(cancellationToken);
        return path;
    }

    public async Task UpdateAsync(LearningPath path, CancellationToken cancellationToken = default)
    {
        _context.LearningPaths.Update(path);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var path = await GetByIdAsync(id, cancellationToken);
        if (path != null)
        {
            path.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

public class LearningPathEnrollmentRepository : ILearningPathEnrollmentRepository
{
    private readonly LMSDbContext _context;

    public LearningPathEnrollmentRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<LearningPathEnrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPathEnrollments
            .Include(e => e.LearningPath)
            .Where(e => !e.IsDeleted && e.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LearningPathEnrollment?> GetByEmployeeAndPathAsync(
        string employeeId,
        Guid pathId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LearningPathEnrollments
            .Include(e => e.LearningPath)
            .Where(e => !e.IsDeleted && e.EmployeeId == employeeId && e.LearningPathId == pathId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<LearningPathEnrollment>> GetByEmployeeAsync(
        string employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LearningPathEnrollments
            .Include(e => e.LearningPath)
                .ThenInclude(p => p.Courses)
            .Where(e => !e.IsDeleted && e.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<LearningPathEnrollment> AddAsync(
        LearningPathEnrollment enrollment,
        CancellationToken cancellationToken = default)
    {
        _context.LearningPathEnrollments.Add(enrollment);
        await _context.SaveChangesAsync(cancellationToken);
        return enrollment;
    }

    public async Task UpdateAsync(
        LearningPathEnrollment enrollment,
        CancellationToken cancellationToken = default)
    {
        _context.LearningPathEnrollments.Update(enrollment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class CoursePrerequisiteRepository : ICoursePrerequisiteRepository
{
    private readonly LMSDbContext _context;

    public CoursePrerequisiteRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<List<CoursePrerequisite>> GetByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CoursePrerequisites
            .Include(p => p.Course)
            .Include(p => p.PrerequisiteCourse)
            .Where(p => !p.IsDeleted && p.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CoursePrerequisite> AddAsync(
        CoursePrerequisite prerequisite,
        CancellationToken cancellationToken = default)
    {
        _context.CoursePrerequisites.Add(prerequisite);
        await _context.SaveChangesAsync(cancellationToken);
        return prerequisite;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prerequisite = await _context.CoursePrerequisites
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (prerequisite != null)
        {
            prerequisite.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
