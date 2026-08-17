using LMS.Application.Services;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Services;
using LMS.Infrastructure.Storage;

namespace LMS.Infrastructure.Services;

public class CertificatePdfService : ICertificatePdfService
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly ICertificateGenerator _certificateGenerator;
    private readonly IBlobStorageService _blobStorage;

    public CertificatePdfService(
        ICertificateRepository certificateRepository,
        ICertificateGenerator certificateGenerator,
        IBlobStorageService blobStorage)
    {
        _certificateRepository = certificateRepository;
        _certificateGenerator = certificateGenerator;
        _blobStorage = blobStorage;
    }

    public async Task<byte[]> GenerateCertificatePdfAsync(Guid certificateId, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetByIdAsync(certificateId, cancellationToken);
        if (certificate == null)
        {
            throw new InvalidOperationException($"Certificate {certificateId} not found");
        }

        return await _certificateGenerator.GenerateCertificatePdfAsync(certificate, cancellationToken);
    }

    public async Task<string> GetCertificatePdfUrlAsync(Guid certificateId, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetByIdAsync(certificateId, cancellationToken);
        if (certificate == null)
        {
            throw new InvalidOperationException($"Certificate {certificateId} not found");
        }

        // Check if PDF already exists
        if (!string.IsNullOrEmpty(certificate.CertificateUrl))
        {
            return certificate.CertificateUrl;
        }

        // Generate PDF
        var pdfBytes = await _certificateGenerator.GenerateCertificatePdfAsync(certificate, cancellationToken);

        // Save to blob storage
        var fileName = $"certificates/{certificate.CertificateNumber}.pdf";
        using var pdfStream = new MemoryStream(pdfBytes);
        var url = await _blobStorage.UploadFileAsync(fileName, pdfStream, "application/pdf", cancellationToken);

        // Update certificate with URL
        certificate.CertificateUrl = url;
        await _certificateRepository.UpdateAsync(certificate, cancellationToken);

        return url;
    }
}
