using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly LMSDbContext _context;

    public QuizRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted, cancellationToken);
    }

    public async Task<Quiz?> GetWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .Where(q => q.CourseId == courseId && !q.IsDeleted)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .Where(q => q.LessonId == lessonId && !q.IsDeleted)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Quiz> AddAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync(cancellationToken);
        return quiz;
    }

    public async Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var quiz = await GetByIdAsync(id, cancellationToken);
        if (quiz != null)
        {
            quiz.IsDeleted = true;
            await UpdateAsync(quiz, cancellationToken);
        }
    }
}
