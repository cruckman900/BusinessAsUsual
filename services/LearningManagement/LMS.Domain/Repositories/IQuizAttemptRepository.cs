using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface IQuizAttemptRepository
{
    Task<QuizAttempt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuizAttempt?> GetWithAnswersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuizAttempt>> GetByEmployeeAndQuizAsync(string employeeId, Guid quizId, CancellationToken cancellationToken = default);
    Task<QuizAttempt?> GetBestAttemptAsync(string employeeId, Guid quizId, CancellationToken cancellationToken = default);
    Task<int> GetAttemptCountAsync(string employeeId, Guid quizId, CancellationToken cancellationToken = default);
    Task<QuizAttempt> AddAsync(QuizAttempt attempt, CancellationToken cancellationToken = default);
    Task UpdateAsync(QuizAttempt attempt, CancellationToken cancellationToken = default);
}

