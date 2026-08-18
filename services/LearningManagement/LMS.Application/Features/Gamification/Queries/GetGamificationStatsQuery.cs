using LMS.Application.Common;
using LMS.Domain.DTOs;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Gamification.Queries;

public class GetGamificationStatsQuery : IQuery<Result<GamificationStatsDto>>
{
    public string EmployeeId { get; set; } = string.Empty;
}

public class GetGamificationStatsQueryHandler : IQueryHandler<GetGamificationStatsQuery, Result<GamificationStatsDto>>
{
    private readonly ILearnerGamificationRepository _gamificationRepository;

    public GetGamificationStatsQueryHandler(ILearnerGamificationRepository gamificationRepository)
    {
        _gamificationRepository = gamificationRepository;
    }

    public async Task<Result<GamificationStatsDto>> HandleAsync(GetGamificationStatsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var stats = await _gamificationRepository.GetByEmployeeIdAsync(query.EmployeeId, cancellationToken);

            if (stats == null)
            {
                // Return default stats for new learner
                return Result<GamificationStatsDto>.Ok(new GamificationStatsDto
                {
                    EmployeeId = query.EmployeeId,
                    TotalPoints = 0,
                    Level = 1,
                    PointsToNextLevel = 100,
                    CoursesCompleted = 0,
                    QuizzesPassed = 0,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    EarnedBadges = new List<BadgeDto>(),
                    AvailableBadges = new List<BadgeDto>()
                });
            }

            var dto = new GamificationStatsDto
            {
                EmployeeId = stats.EmployeeId,
                TotalPoints = stats.TotalPoints,
                Level = stats.Level,
                PointsToNextLevel = CalculatePointsToNextLevel(stats.Level, stats.TotalPoints),
                CoursesCompleted = stats.CoursesCompleted,
                QuizzesPassed = stats.QuizzesPassed,
                CurrentStreak = stats.CurrentStreak,
                LongestStreak = stats.LongestStreak,
                EarnedBadges = stats.Badges.Select(b => new BadgeDto
                {
                    Id = b.Badge.Id,
                    Name = b.Badge.Name,
                    Description = b.Badge.Description,
                    IconUrl = b.Badge.IconUrl,
                    Type = b.Badge.Type.ToString(),
                    IsEarned = true,
                    EarnedAt = b.EarnedAt
                }).ToList(),
                AvailableBadges = new List<BadgeDto>() // TODO: Load available badges
            };

            return Result<GamificationStatsDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<GamificationStatsDto>.Fail($"Failed to retrieve gamification stats: {ex.Message}");
        }
    }

    private int CalculatePointsToNextLevel(int currentLevel, int totalPoints)
    {
        // Simple level progression: Level 1 = 100 points, Level 2 = 250, Level 3 = 500, etc.
        var pointsForNextLevel = currentLevel * 100 + (currentLevel - 1) * 50;
        var pointsForCurrentLevel = (currentLevel - 1) * 100 + (currentLevel - 2) * 50;
        return pointsForNextLevel - (totalPoints - pointsForCurrentLevel);
    }
}
