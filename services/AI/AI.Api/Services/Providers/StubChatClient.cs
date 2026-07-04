using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace AI.Api.Services.Providers;

/// <summary>
/// Development-only <see cref="IChatClient"/> that returns a canned "paid tier" response.
/// It exists so the two-tier routing can be verified end-to-end without calling Amazon
/// Bedrock (and therefore at zero AWS cost). Never registered outside Development.
/// </summary>
public sealed class StubChatClient : IChatClient
{
    private const string StubModelId = "stub-paid-model";

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);

        var lastUser = messages.LastOrDefault(m => m.Role == ChatRole.User)?.Text ?? string.Empty;
        var text =
            "[PAID TIER STUB] Routing to the paid tier succeeded. " +
            "Swap this stub for Amazon Bedrock (Claude Haiku) to get real answers. " +
            $"You asked: \"{lastUser}\"";

        var response = new ChatResponse(new ChatMessage(ChatRole.Assistant, text))
        {
            ModelId = StubModelId,
            FinishReason = ChatFinishReason.Stop
        };

        return Task.FromResult(response);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(messages, options, cancellationToken).ConfigureAwait(false);
        yield return new ChatResponseUpdate(ChatRole.Assistant, response.Text);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        return serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;
    }

    public void Dispose()
    {
        // Nothing to dispose.
    }
}
