namespace AI.Api.Services;

/// <summary>
/// Resolves whether a given company identifier belongs to a real, provisioned company.
/// Used to gate access to the paid AI tier.
/// </summary>
public interface ICompanyDirectory
{
    /// <summary>
    /// Returns company info if the supplied id maps to a provisioned company; otherwise null.
    /// </summary>
    /// <param name="companyId">The company identifier supplied by the caller.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<CompanyRecord?> FindCompanyAsync(Guid companyId, CancellationToken ct = default);
}

/// <summary>Lightweight projection of a provisioned company record.</summary>
/// <param name="Id">Company identifier.</param>
/// <param name="Name">Company display name.</param>
/// <param name="BillingPlan">Billing plan assigned at provisioning time.</param>
public sealed record CompanyRecord(Guid Id, string Name, string BillingPlan);
