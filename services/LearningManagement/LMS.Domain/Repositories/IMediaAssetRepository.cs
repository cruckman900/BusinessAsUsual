using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface IMediaAssetRepository
{
    Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MediaAsset>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MediaAsset>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MediaAsset> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default);
    Task UpdateAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
