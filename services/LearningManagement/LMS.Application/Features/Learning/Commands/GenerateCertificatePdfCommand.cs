using LMS.Application.Common;
using LMS.Application.Services;

namespace LMS.Application.Features.Learning.Commands;

public class GenerateCertificatePdfCommand : ICommand<Result<string>>
{
    public Guid CertificateId { get; set; }
}

public class GenerateCertificatePdfCommandHandler : ICommandHandler<GenerateCertificatePdfCommand, Result<string>>
{
    private readonly ICertificatePdfService _pdfService;

    public GenerateCertificatePdfCommandHandler(ICertificatePdfService pdfService)
    {
        _pdfService = pdfService;
    }

    public async Task<Result<string>> HandleAsync(
        GenerateCertificatePdfCommand command, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = await _pdfService.GetCertificatePdfUrlAsync(command.CertificateId, cancellationToken);
            return Result<string>.Ok(url);
        }
        catch (Exception ex)
        {
            return Result<string>.Fail($"Failed to generate certificate PDF: {ex.Message}");
        }
    }
}
