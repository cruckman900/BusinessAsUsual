using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Learning.Commands;

public class SubmitQuizAttemptCommandHandler : ICommandHandler<SubmitQuizAttemptCommand, Result<QuizAttemptResult>>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public SubmitQuizAttemptCommandHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<Result<QuizAttemptResult>> HandleAsync(
        SubmitQuizAttemptCommand command, 
        CancellationToken cancellationToken = default)
    {
        // Get the quiz with questions and options
        var quiz = await _quizRepository.GetWithQuestionsAsync(command.QuizId, cancellationToken);
        if (quiz == null)
        {
            return Result<QuizAttemptResult>.Fail("Quiz not found");
        }

        // Get previous attempts
        var previousAttempts = await _quizAttemptRepository.GetByEmployeeAndQuizAsync(
            command.EmployeeId, 
            command.QuizId, 
            cancellationToken);

        var attemptNumber = previousAttempts.Count() + 1;

        // Check if max attempts exceeded
        if (quiz.MaxAttempts > 0 && attemptNumber > quiz.MaxAttempts)
        {
            return Result<QuizAttemptResult>.Fail($"Maximum attempts ({quiz.MaxAttempts}) exceeded");
        }

        // Create the quiz attempt
        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = command.QuizId,
            Quiz = quiz,
            EmployeeId = command.EmployeeId,
            AttemptNumber = attemptNumber,
            StartedAt = command.StartedAt,
            CompletedAt = command.CompletedAt,
            Status = QuizAttemptStatus.Completed,
            Answers = new List<Answer>(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = command.EmployeeId
        };

        // Grade each answer
        int totalPoints = 0;
        int earnedPoints = 0;

        foreach (var question in quiz.Questions)
        {
            totalPoints += question.Points;

            if (!command.Answers.TryGetValue(question.Id, out var studentAnswer))
            {
                // Question not answered
                attempt.Answers.Add(new Answer
                {
                    Id = Guid.NewGuid(),
                    QuizAttemptId = attempt.Id,
                    QuestionId = question.Id,
                    Question = question,
                    IsCorrect = false,
                    PointsEarned = 0,
                    CreatedAt = DateTime.UtcNow
                });
                continue;
            }

            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                QuizAttemptId = attempt.Id,
                QuestionId = question.Id,
                Question = question,
                SelectedOptionId = studentAnswer.SelectedOptionId,
                SelectedOptionIds = studentAnswer.SelectedOptionIds ?? new List<Guid>(),
                TextAnswer = studentAnswer.TextAnswer,
                CreatedAt = DateTime.UtcNow
            };

            // Grade based on question type
            if (question.Type == QuestionType.MultipleChoice || question.Type == QuestionType.TrueFalse)
            {
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null && answer.SelectedOptionId == correctOption.Id)
                {
                    answer.IsCorrect = true;
                    answer.PointsEarned = question.Points;
                    earnedPoints += question.Points;
                }
                else
                {
                    answer.IsCorrect = false;
                    answer.PointsEarned = 0;
                }
            }
            else if (question.Type == QuestionType.MultipleSelect)
            {
                var correctOptionIds = question.Options
                    .Where(o => o.IsCorrect)
                    .Select(o => o.Id)
                    .OrderBy(id => id)
                    .ToList();

                var selectedIds = answer.SelectedOptionIds
                    .OrderBy(id => id)
                    .ToList();

                if (correctOptionIds.SequenceEqual(selectedIds))
                {
                    answer.IsCorrect = true;
                    answer.PointsEarned = question.Points;
                    earnedPoints += question.Points;
                }
                else
                {
                    answer.IsCorrect = false;
                    answer.PointsEarned = 0;
                }
            }
            else if (question.Type == QuestionType.ShortAnswer)
            {
                // Short answer requires manual grading for now
                // Could implement keyword matching or AI grading in the future
                answer.IsCorrect = false;
                answer.PointsEarned = 0;
                // TODO: Flag for manual review
            }

            attempt.Answers.Add(answer);
        }

        // Calculate final score
        attempt.TotalPoints = totalPoints;
        attempt.PointsEarned = earnedPoints;
        attempt.ScorePercentage = totalPoints > 0 ? (decimal)earnedPoints / totalPoints * 100 : 0;
        attempt.Passed = attempt.ScorePercentage >= quiz.PassingScore;

        // Save the attempt
        await _quizAttemptRepository.AddAsync(attempt, cancellationToken);

        var result = new QuizAttemptResult
        {
            AttemptId = attempt.Id,
            AttemptNumber = attempt.AttemptNumber,
            TotalPoints = attempt.TotalPoints,
            PointsEarned = attempt.PointsEarned,
            ScorePercentage = attempt.ScorePercentage,
            Passed = attempt.Passed,
            Attempt = attempt
        };

        return Result<QuizAttemptResult>.Ok(result);
    }
}
