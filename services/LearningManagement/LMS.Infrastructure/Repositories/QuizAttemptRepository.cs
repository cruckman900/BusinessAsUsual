using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly LMSDbContext _context;

    public QuizAttemptRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<QuizAttempt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .Include(a => a.Quiz)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    public async Task<QuizAttempt?> GetWithAnswersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .Include(a => a.Quiz)
            .Include(a => a.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<QuizAttempt>> GetByEmployeeAndQuizAsync(string employeeId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .Include(a => a.Quiz)
            .Include(a => a.Answers)
            .Where(a => a.EmployeeId == employeeId && a.QuizId == quizId && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuizAttempt?> GetBestAttemptAsync(string employeeId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .Where(a => a.EmployeeId == employeeId && a.QuizId == quizId && a.Status == QuizAttemptStatus.Completed && !a.IsDeleted)
            .OrderByDescending(a => a.ScorePercentage)
            .ThenByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetAttemptCountAsync(string employeeId, Guid quizId, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .CountAsync(a => a.EmployeeId == employeeId && a.QuizId == quizId && !a.IsDeleted, cancellationToken);
    }

    public async Task<QuizAttempt> AddAsync(QuizAttempt attempt, CancellationToken cancellationToken = default)
    {
        _context.QuizAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);
        return attempt;
    }

    public async Task UpdateAsync(QuizAttempt attempt, CancellationToken cancellationToken = default)
    {
        _context.QuizAttempts.Update(attempt);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
