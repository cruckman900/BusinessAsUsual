using AI.Api.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AI.Api.Services;

/// <summary>
/// Default orchestrator. Resolves the tier (valid provisioned companyId => paid, else demo),
/// enforces input/output caps, injects the system prompt, and calls the resolved chat client.
/// </summary>
public sealed class AiChatService : IAiChatService
{
    private readonly AiClientRegistry _clients;
    private readonly ICompanyDirectory _companies;
    private readonly AiOptions _options;
    private readonly ILogger<AiChatService> _logger;

    public AiChatService(
        AiClientRegistry clients,
        ICompanyDirectory companies,
        IOptions<AiOptions> options,
        ILogger<AiChatService> logger)
    {
        _clients = clients;
        _companies = companies;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AiChatResult> AskAsync(AiChatRequest request, CancellationToken ct = default)
    {
        var query = request.Query?.Trim() ?? string.Empty;

        // --- Guardrail: input presence + length cap ---
        if (string.IsNullOrWhiteSpace(query))
        {
            return new AiChatResult("Please enter a question.", AiTier.Demo.ToString(), null);
        }

        if (query.Length > _options.MaxInputChars)
        {
            return new AiChatResult(
                $"Your message is too long (limit {_options.MaxInputChars} characters). Please shorten it.",
                AiTier.Demo.ToString(),
                null);
        }

        // --- Tier resolution: only a real provisioned company unlocks the paid tier ---
        var tier = await ResolveTierAsync(request.CompanyId, ct).ConfigureAwait(false);

        var client = _clients.For(tier);
        if (client is null && tier == AiTier.Paid)
        {
            // Paid unavailable (not configured) -> gracefully fall back to demo.
            _logger.LogWarning("Paid tier requested but not configured; falling back to demo tier.");
            tier = AiTier.Demo;
            client = _clients.For(tier);
        }

        if (client is null)
        {
            return new AiChatResult(
                "The AI service is not configured yet. Please set up a provider and try again.",
                tier.ToString(),
                null);
        }

        // --- Build the conversation with the system prompt + output cap ---
        var messages = new List<ChatMessage>();
        if (!string.IsNullOrWhiteSpace(_options.SystemPrompt))
        {
            messages.Add(new ChatMessage(ChatRole.System, _options.SystemPrompt));
        }
        messages.Add(new ChatMessage(ChatRole.User, query));

        var chatOptions = new ChatOptions
        {
            MaxOutputTokens = _options.MaxOutputTokens
        };

        try
        {
            // Fail fast if the provider is slow/unresponsive instead of hanging
            // until the web caller times out. Link the caller's token with an
            // internal timeout so either can cancel the provider call.
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(1, _options.RequestTimeoutSeconds)));

            var response = await client.GetResponseAsync(messages, chatOptions, timeoutCts.Token).ConfigureAwait(false);
            var answer = response.Text;
            if (string.IsNullOrWhiteSpace(answer))
            {
                answer = "I couldn't generate a response. Please try rephrasing.";
            }

            return new AiChatResult(answer, tier.ToString(), response.ModelId);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            // Our internal timeout tripped (the caller did not cancel).
            _logger.LogWarning(
                "AI provider call timed out after {Timeout}s for tier {Tier}.",
                _options.RequestTimeoutSeconds, tier);
            return new AiChatResult(
                "The AI service is taking longer than usual right now. Please try again in a moment.",
                tier.ToString(),
                null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI provider call failed for tier {Tier}.", tier);
            return new AiChatResult(
                "Something went wrong while contacting the AI provider. Please try again later.",
                tier.ToString(),
                null);
        }
    }

    private async Task<AiTier> ResolveTierAsync(Guid? companyId, CancellationToken ct)
    {
        if (companyId is null || companyId.Value == Guid.Empty)
        {
            return AiTier.Demo;
        }

        var company = await _companies.FindCompanyAsync(companyId.Value, ct).ConfigureAwait(false);
        if (company is null)
        {
            _logger.LogInformation("CompanyId {CompanyId} not found in provisioning DB; using demo tier.", companyId);
            return AiTier.Demo;
        }

        _logger.LogInformation("CompanyId {CompanyId} ({Name}) validated; using paid tier.", company.Id, company.Name);
        return AiTier.Paid;
    }
}
