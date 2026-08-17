using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly LMSDbContext _context;

    public AssignmentRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .Include(a => a.Course)
            .Where(a => a.EmployeeId == employeeId && !a.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetOverdueAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Assignments
            .Include(a => a.Course)
            .Where(a => a.DueDate.HasValue && a.DueDate < now && a.Status != AssignmentStatus.Completed && !a.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Assignment> AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        return assignment;
    }

    public async Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetByIdAsync(id, cancellationToken);
        if (assignment != null)
        {
            assignment.IsDeleted = true;
            await UpdateAsync(assignment, cancellationToken);
        }
    }
}
