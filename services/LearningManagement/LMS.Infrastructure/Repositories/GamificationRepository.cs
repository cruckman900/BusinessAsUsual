using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class LearnerGamificationRepository : ILearnerGamificationRepository
{
    private readonly LMSDbContext _context;

    public LearnerGamificationRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<LearnerGamification?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.LearnerGamifications
            .Include(g => g.Badges)
                .ThenInclude(b => b.Badge)
            .Where(g => !g.IsDeleted && g.EmployeeId == employeeId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LearnerGamification> AddAsync(LearnerGamification gamification, CancellationToken cancellationToken = default)
    {
        _context.LearnerGamifications.Add(gamification);
        await _context.SaveChangesAsync(cancellationToken);
        return gamification;
    }

    public async Task UpdateAsync(LearnerGamification gamification, CancellationToken cancellationToken = default)
    {
        _context.LearnerGamifications.Update(gamification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<LearnerGamification>> GetLeaderboardAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.LearnerGamifications
            .Where(g => !g.IsDeleted)
            .OrderByDescending(g => g.TotalPoints)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}

public class BadgeRepository : IBadgeRepository
{
    private readonly LMSDbContext _context;

    public BadgeRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<Badge?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Badges
            .Where(b => !b.IsDeleted && b.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Badge>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Badges
            .Where(b => !b.IsDeleted && b.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Badge> AddAsync(Badge badge, CancellationToken cancellationToken = default)
    {
        _context.Badges.Add(badge);
        await _context.SaveChangesAsync(cancellationToken);
        return badge;
    }
}

public class EarnedBadgeRepository : IEarnedBadgeRepository
{
    private readonly LMSDbContext _context;

    public EarnedBadgeRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<List<EarnedBadge>> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.EarnedBadges
            .Include(e => e.Badge)
            .Where(e => !e.IsDeleted && e.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<EarnedBadge> AddAsync(EarnedBadge earnedBadge, CancellationToken cancellationToken = default)
    {
        _context.EarnedBadges.Add(earnedBadge);
        await _context.SaveChangesAsync(cancellationToken);
        return earnedBadge;
    }
}
