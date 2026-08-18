using System.Text;
using LMS.Application.Services;
using LMS.Domain.DTOs;

namespace LMS.Infrastructure.Services;

/// <summary>
/// CSV report generation service for quiz analytics
/// </summary>
public class CsvReportGenerationService : IReportGenerationService
{
    public Task<byte[]> GenerateQuizAnalyticsReportAsync(
        QuizAnalyticsDto analytics,
        CancellationToken cancellationToken = default)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine($"Quiz Analytics Report");
        csv.AppendLine($"Quiz: {EscapeCsv(analytics.QuizTitle)}");
        csv.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        csv.AppendLine();

        // Overall Statistics
        csv.AppendLine("Overall Statistics");
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Attempts,{analytics.TotalAttempts}");
        csv.AppendLine($"Unique Learners,{analytics.UniqueLearnersAttempted}");
        csv.AppendLine($"Completed Attempts,{analytics.CompletedAttempts}");
        csv.AppendLine($"In Progress,{analytics.InProgressAttempts}");
        csv.AppendLine($"Abandoned,{analytics.AbandonedAttempts}");
        csv.AppendLine();

        // Performance Metrics
        csv.AppendLine("Performance Metrics");
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Average Score,{analytics.AverageScore}%");
        csv.AppendLine($"Median Score,{analytics.MedianScore}%");
        csv.AppendLine($"Highest Score,{analytics.HighestScore}%");
        csv.AppendLine($"Lowest Score,{analytics.LowestScore}%");
        csv.AppendLine($"Pass Count,{analytics.PassCount}");
        csv.AppendLine($"Fail Count,{analytics.FailCount}");
        csv.AppendLine($"Pass Rate,{analytics.PassRate}%");
        csv.AppendLine($"First Attempt Pass Rate,{analytics.FirstAttemptPassRate}%");
        csv.AppendLine();

        // Timing Metrics
        csv.AppendLine("Timing Metrics");
        csv.AppendLine("Metric,Value (Minutes)");
        csv.AppendLine($"Average Completion Time,{analytics.AverageCompletionTimeMinutes}");
        csv.AppendLine($"Median Completion Time,{analytics.MedianCompletionTimeMinutes}");
        csv.AppendLine($"Fastest Completion,{analytics.FastestCompletionTimeMinutes}");
        csv.AppendLine($"Slowest Completion,{analytics.SlowestCompletionTimeMinutes}");
        csv.AppendLine();

        // Question Metrics
        if (analytics.QuestionMetrics.Any())
        {
            csv.AppendLine("Question-Level Metrics");
            csv.AppendLine("Question,Type,Total Answers,Correct,Incorrect,Success Rate %,Avg Points,Difficulty");
            foreach (var q in analytics.QuestionMetrics.OrderBy(q => q.OrderIndex))
            {
                csv.AppendLine($"\"{EscapeCsv(q.QuestionText)}\",{q.QuestionType}," +
                              $"{q.TotalAnswers},{q.CorrectAnswers},{q.IncorrectAnswers}," +
                              $"{q.SuccessRate},{q.AveragePointsEarned},{q.PerceivedDifficulty}");
            }
            csv.AppendLine();
        }

        // Attempt Distribution
        if (analytics.AttemptDistribution.Any())
        {
            csv.AppendLine("Attempt Distribution");
            csv.AppendLine("Attempt Number,Count");
            foreach (var kvp in analytics.AttemptDistribution.OrderBy(x => x.Key))
            {
                csv.AppendLine($"{kvp.Key},{kvp.Value}");
            }
            csv.AppendLine();
        }

        // Daily Metrics
        if (analytics.DailyMetrics.Any())
        {
            csv.AppendLine("Daily Trends");
            csv.AppendLine("Date,Attempts,Completions,Avg Score %,Pass Rate %");
            foreach (var day in analytics.DailyMetrics.OrderBy(d => d.Date))
            {
                csv.AppendLine($"{day.Date:yyyy-MM-dd},{day.AttemptCount}," +
                              $"{day.CompletedCount},{day.AverageScore},{day.PassRate}");
            }
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    public Task<byte[]> GenerateLearnerHistoryReportAsync(
        List<LearnerQuizHistoryDto> history,
        CancellationToken cancellationToken = default)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine($"Learner Quiz History Report");
        csv.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        csv.AppendLine();

        // Summary per quiz
        csv.AppendLine("Quiz Summary");
        csv.AppendLine("Employee ID,Employee Name,Quiz,Total Attempts,Best Score %,Latest Score %,Avg Score %,Passed");
        foreach (var h in history)
        {
            csv.AppendLine($"{h.EmployeeId},\"{EscapeCsv(h.EmployeeName)}\"," +
                          $"\"{EscapeCsv(h.QuizTitle)}\",{h.TotalAttempts}," +
                          $"{h.BestScore},{h.LatestScore},{h.AverageScore},{h.HasPassed}");
        }
        csv.AppendLine();

        // Detailed attempts
        foreach (var h in history)
        {
            csv.AppendLine($"Detailed Attempts - {EscapeCsv(h.QuizTitle)}");
            csv.AppendLine("Attempt #,Started,Completed,Score %,Passed,Points Earned,Total Points,Status,Time (min)");
            foreach (var attempt in h.Attempts.OrderBy(a => a.AttemptNumber))
            {
                csv.AppendLine($"{attempt.AttemptNumber}," +
                              $"{attempt.StartedAt:yyyy-MM-dd HH:mm}," +
                              $"{attempt.CompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}," +
                              $"{attempt.ScorePercentage},{attempt.Passed}," +
                              $"{attempt.PointsEarned},{attempt.TotalPoints}," +
                              $"{attempt.Status},{attempt.CompletionTimeMinutes?.ToString("F2") ?? "N/A"}");
            }
            csv.AppendLine();

            if (h.WeakAreas.Any())
            {
                csv.AppendLine($"Weak Areas - {EscapeCsv(h.QuizTitle)}");
                foreach (var area in h.WeakAreas)
                {
                    csv.AppendLine($"\"{EscapeCsv(area)}\"");
                }
                csv.AppendLine();
            }
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    public Task<byte[]> GeneratePerformanceSummaryReportAsync(
        QuizPerformanceSummaryDto summary,
        CancellationToken cancellationToken = default)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine($"Quiz Performance Summary Report");
        csv.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        csv.AppendLine();

        // Overall Metrics
        csv.AppendLine("Overall Metrics");
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Quizzes,{summary.TotalQuizzes}");
        csv.AppendLine($"Total Attempts,{summary.TotalAttempts}");
        csv.AppendLine($"Total Completions,{summary.TotalCompletions}");
        csv.AppendLine($"Overall Average Score,{summary.OverallAverageScore}%");
        csv.AppendLine($"Overall Pass Rate,{summary.OverallPassRate}%");
        csv.AppendLine($"Active Learners,{summary.ActiveLearners}");
        csv.AppendLine();

        // Top Performing Quizzes
        if (summary.TopPerformingQuizzes.Any())
        {
            csv.AppendLine("Top Performing Quizzes (by Average Score)");
            csv.AppendLine("Quiz,Attempts,Avg Score %,Pass Rate %");
            foreach (var quiz in summary.TopPerformingQuizzes)
            {
                csv.AppendLine($"\"{EscapeCsv(quiz.Title)}\",{quiz.AttemptCount}," +
                              $"{quiz.AverageScore},{quiz.PassRate}");
            }
            csv.AppendLine();
        }

        // Lowest Performing Quizzes
        if (summary.LowestPerformingQuizzes.Any())
        {
            csv.AppendLine("Lowest Performing Quizzes (by Pass Rate)");
            csv.AppendLine("Quiz,Attempts,Avg Score %,Pass Rate %");
            foreach (var quiz in summary.LowestPerformingQuizzes)
            {
                csv.AppendLine($"\"{EscapeCsv(quiz.Title)}\",{quiz.AttemptCount}," +
                              $"{quiz.AverageScore},{quiz.PassRate}");
            }
            csv.AppendLine();
        }

        // Most Attempted Quizzes
        if (summary.MostAttemptedQuizzes.Any())
        {
            csv.AppendLine("Most Attempted Quizzes");
            csv.AppendLine("Quiz,Attempts,Avg Score %,Pass Rate %");
            foreach (var quiz in summary.MostAttemptedQuizzes)
            {
                csv.AppendLine($"\"{EscapeCsv(quiz.Title)}\",{quiz.AttemptCount}," +
                              $"{quiz.AverageScore},{quiz.PassRate}");
            }
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    public Task<byte[]> GenerateQuestionMetricsReportAsync(
        List<QuestionMetricsDto> metrics,
        string quizTitle,
        CancellationToken cancellationToken = default)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine($"Question Metrics Report");
        csv.AppendLine($"Quiz: {EscapeCsv(quizTitle)}");
        csv.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        csv.AppendLine();

        // Question Metrics
        csv.AppendLine("Question Metrics");
        csv.AppendLine("Order,Question,Type,Total Answers,Correct,Incorrect,Success Rate %,Avg Points,Max Points,Difficulty");
        foreach (var q in metrics.OrderBy(q => q.OrderIndex))
        {
            csv.AppendLine($"{q.OrderIndex},\"{EscapeCsv(q.QuestionText)}\",{q.QuestionType}," +
                          $"{q.TotalAnswers},{q.CorrectAnswers},{q.IncorrectAnswers}," +
                          $"{q.SuccessRate},{q.AveragePointsEarned},{q.MaxPoints},{q.PerceivedDifficulty}");
        }
        csv.AppendLine();

        // Common Wrong Answers (for questions that have them)
        var questionsWithWrongAnswers = metrics
            .Where(q => q.CommonWrongAnswers.Any())
            .ToList();

        if (questionsWithWrongAnswers.Any())
        {
            csv.AppendLine("Common Wrong Answers");
            csv.AppendLine("Question,Wrong Answer,Selection Count,Selection Rate %");
            foreach (var q in questionsWithWrongAnswers)
            {
                foreach (var wrong in q.CommonWrongAnswers)
                {
                    csv.AppendLine($"\"{EscapeCsv(q.QuestionText)}\",\"{EscapeCsv(wrong.OptionText)}\"," +
                                  $"{wrong.SelectionCount},{wrong.SelectionRate}");
                }
            }
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    private string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // Replace quotes with double quotes and handle commas/newlines
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n'))
        {
            return value.Replace("\"", "\"\"");
        }

        return value;
    }

    public Task<byte[]> GeneratePdfReportAsync(GenerateReportRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Implement PDF generation using a library like QuestPDF or iTextSharp
        // For now, return a placeholder message as UTF-8 bytes
        var message = $"PDF Report Generation\nReport Type: {request.ReportType}\nDate Range: {request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}\n\nPDF generation requires additional NuGet package (QuestPDF, iTextSharp, etc.)";
        return Task.FromResult(Encoding.UTF8.GetBytes(message));
    }

    public Task<byte[]> GenerateLearningAnalyticsPdfAsync(LearningAnalyticsDashboardDto dashboard, CancellationToken cancellationToken = default)
    {
        // TODO: Implement PDF generation for learning analytics dashboard
        // For now, return a placeholder message as UTF-8 bytes
        var message = $"Learning Analytics Dashboard PDF\nGenerated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n\nTotal Enrollments: {dashboard.OverallMetrics.TotalEnrollments}\nActive Learners: {dashboard.OverallMetrics.ActiveLearners}\nCompletion Rate: {dashboard.OverallMetrics.OverallCompletionRate:F1}%\n\nPDF generation requires additional NuGet package (QuestPDF, iTextSharp, etc.)";
        return Task.FromResult(Encoding.UTF8.GetBytes(message));
    }
}
