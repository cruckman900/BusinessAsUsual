using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Learning.Queries;

public class GetQuizAttemptsQuery : IQuery<List<QuizAttempt>>
{
    public string EmployeeId { get; set; } = string.Empty;
    public Guid QuizId { get; set; }
}

public class GetQuizAttemptsQueryHandler : IQueryHandler<GetQuizAttemptsQuery, List<QuizAttempt>>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public GetQuizAttemptsQueryHandler(IQuizAttemptRepository quizAttemptRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<List<QuizAttempt>> HandleAsync(
        GetQuizAttemptsQuery query, 
        CancellationToken cancellationToken = default)
    {
        var attempts = await _quizAttemptRepository.GetByEmployeeAndQuizAsync(
            query.EmployeeId, 
            query.QuizId, 
            cancellationToken);

        return attempts.OrderByDescending(a => a.CreatedAt).ToList();
    }
}
