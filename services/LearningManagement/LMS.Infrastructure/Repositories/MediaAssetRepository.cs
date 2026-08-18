using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class MediaAssetRepository : IMediaAssetRepository
{
    private readonly LMSDbContext _context;

    public MediaAssetRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MediaAssets
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MediaAsset>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.MediaAssets
            .Where(m => m.CourseId == courseId)
            .OrderByDescending(m => m.UploadedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MediaAsset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MediaAssets
            .Include(m => m.Course)
            .OrderByDescending(m => m.UploadedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<MediaAsset> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default)
    {
        _context.MediaAssets.Add(mediaAsset);
        await _context.SaveChangesAsync(cancellationToken);
        return mediaAsset;
    }

    public async Task UpdateAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default)
    {
        _context.MediaAssets.Update(mediaAsset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mediaAsset = await GetByIdAsync(id, cancellationToken);
        if (mediaAsset != null)
        {
            _context.MediaAssets.Remove(mediaAsset);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
