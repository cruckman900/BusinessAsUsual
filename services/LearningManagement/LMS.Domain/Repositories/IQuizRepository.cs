using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface IQuizRepository
{
    Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Quiz?> GetWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Quiz?> GetWithAttemptsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Quiz>> GetAllWithAttemptsAsync(CancellationToken cancellationToken = default);
    Task<Quiz> AddAsync(Quiz quiz, CancellationToken cancellationToken = default);
    Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

