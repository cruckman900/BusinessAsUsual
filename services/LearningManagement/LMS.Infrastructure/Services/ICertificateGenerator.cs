using LMS.Domain.Entities;

namespace LMS.Infrastructure.Services;

public interface ICertificateGenerator
{
    Task<byte[]> GenerateCertificatePdfAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task<string> SaveCertificatePdfAsync(Certificate certificate, string outputPath, CancellationToken cancellationToken = default);
}
