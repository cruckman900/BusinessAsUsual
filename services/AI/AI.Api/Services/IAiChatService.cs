namespace AI.Api.Services;

/// <summary>Input to the AI chat service.</summary>
/// <param name="Query">The user's prompt.</param>
/// <param name="CompanyId">Optional company id; a valid provisioned id unlocks the paid tier.</param>
public sealed record AiChatRequest(string Query, Guid? CompanyId);

/// <summary>Result of an AI chat call.</summary>
/// <param name="Answer">The model's answer (or a user-facing error message).</param>
/// <param name="Tier">Which tier served the request.</param>
/// <param name="Model">The model id that produced the answer, if known.</param>
public sealed record AiChatResult(string Answer, string Tier, string? Model);

/// <summary>Orchestrates tier resolution, guardrails, and the underlying chat call.</summary>
public interface IAiChatService
{
    Task<AiChatResult> AskAsync(AiChatRequest request, CancellationToken ct = default);
}
