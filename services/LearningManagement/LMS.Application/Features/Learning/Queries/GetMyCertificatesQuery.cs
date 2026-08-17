using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Learning.Queries;

public class GetMyCertificatesQuery : IQuery<List<Certificate>>
{
    public string EmployeeId { get; set; } = string.Empty;
    public bool ActiveOnly { get; set; } = false;
}

public class GetMyCertificatesQueryHandler : IQueryHandler<GetMyCertificatesQuery, List<Certificate>>
{
    private readonly ICertificateRepository _certificateRepository;

    public GetMyCertificatesQueryHandler(ICertificateRepository certificateRepository)
    {
        _certificateRepository = certificateRepository;
    }

    public async Task<List<Certificate>> HandleAsync(
        GetMyCertificatesQuery query, 
        CancellationToken cancellationToken = default)
    {
        var certificates = await _certificateRepository.GetByEmployeeAsync(query.EmployeeId, cancellationToken);

        if (query.ActiveOnly)
        {
            certificates = certificates.Where(c => c.Status == CertificateStatus.Active).ToList();
        }

        return certificates.OrderByDescending(c => c.IssuedDate).ToList();
    }
}
