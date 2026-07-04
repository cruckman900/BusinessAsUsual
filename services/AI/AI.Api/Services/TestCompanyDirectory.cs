using AI.Api.Configuration;
using Microsoft.Extensions.Options;

namespace AI.Api.Services;

/// <summary>
/// Development-only <see cref="ICompanyDirectory"/> that treats a configured set of test
/// company ids as provisioned, without touching the provisioning database. This lets the
/// paid-tier routing be exercised locally without a real company id. Any id it does not
/// recognize is delegated to an inner directory (the real SQL one), so normal validation
/// still works in development. Never registered outside Development.
/// </summary>
public sealed class TestCompanyDirectory : ICompanyDirectory
{
    private readonly HashSet<Guid> _testCompanyIds;
    private readonly ICompanyDirectory? _fallback;
    private readonly ILogger<TestCompanyDirectory> _logger;

    public TestCompanyDirectory(
        IOptions<AiOptions> options,
        ILogger<TestCompanyDirectory> logger,
        ICompanyDirectory? fallback = null)
    {
        _logger = logger;
        _fallback = fallback;
        _testCompanyIds = new HashSet<Guid>();

        foreach (var raw in options.Value.Dev.TestCompanyIds ?? Array.Empty<string>())
        {
            if (Guid.TryParse(raw, out var id) && id != Guid.Empty)
            {
                _testCompanyIds.Add(id);
            }
        }
    }

    public async Task<CompanyRecord?> FindCompanyAsync(Guid companyId, CancellationToken ct = default)
    {
        if (companyId != Guid.Empty && _testCompanyIds.Contains(companyId))
        {
            _logger.LogInformation(
                "TestCompanyDirectory matched dev test company {CompanyId}; unlocking paid tier without the database.",
                companyId);

            return new CompanyRecord(companyId, "Test Company (dev)", "test");
        }

        if (_fallback is not null)
        {
            return await _fallback.FindCompanyAsync(companyId, ct).ConfigureAwait(false);
        }

        return null;
    }
}
