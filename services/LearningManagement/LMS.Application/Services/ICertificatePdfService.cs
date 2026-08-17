namespace LMS.Application.Services;

public interface ICertificatePdfService
{
    Task<byte[]> GenerateCertificatePdfAsync(Guid certificateId, CancellationToken cancellationToken = default);
    Task<string> GetCertificatePdfUrlAsync(Guid certificateId, CancellationToken cancellationToken = default);
}
