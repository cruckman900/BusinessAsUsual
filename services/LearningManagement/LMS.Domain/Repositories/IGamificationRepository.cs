using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface ILearnerGamificationRepository
{
    Task<LearnerGamification?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<LearnerGamification> AddAsync(LearnerGamification gamification, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearnerGamification gamification, CancellationToken cancellationToken = default);
    Task<List<LearnerGamification>> GetLeaderboardAsync(int count = 10, CancellationToken cancellationToken = default);
}

public interface IBadgeRepository
{
    Task<Badge?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Badge>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<Badge> AddAsync(Badge badge, CancellationToken cancellationToken = default);
}

public interface IEarnedBadgeRepository
{
    Task<List<EarnedBadge>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<EarnedBadge> AddAsync(EarnedBadge earnedBadge, CancellationToken cancellationToken = default);
}
