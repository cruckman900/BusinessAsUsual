using System.Runtime.CompilerServices;
using AI.Api.Services;
using Microsoft.Extensions.AI;

namespace AI.Tests.Fakes;

/// <summary>
/// Configurable <see cref="IChatClient"/> test double. Records the messages/options it
/// receives, returns a canned response, or throws when <see cref="ThrowOnCall"/> is set.
/// </summary>
public sealed class FakeChatClient : IChatClient
{
    private readonly string _responseText;
    private readonly string _modelId;

    public FakeChatClient(string responseText = "fake answer", string modelId = "fake-model")
    {
        _responseText = responseText;
        _modelId = modelId;
    }

    public bool ThrowOnCall { get; set; }
    public int CallCount { get; private set; }
    public IReadOnlyList<ChatMessage>? LastMessages { get; private set; }
    public ChatOptions? LastOptions { get; private set; }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        CallCount++;
        LastMessages = messages.ToList();
        LastOptions = options;

        if (ThrowOnCall)
        {
            throw new InvalidOperationException("Simulated provider failure.");
        }

        var response = new ChatResponse(new ChatMessage(ChatRole.Assistant, _responseText))
        {
            ModelId = _modelId,
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

    public object? GetService(Type serviceType, object? serviceKey = null) =>
        serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;

    public void Dispose()
    {
    }
}

/// <summary>
/// Test <see cref="ICompanyDirectory"/> that recognizes a fixed set of provisioned company ids.
/// </summary>
public sealed class FakeCompanyDirectory : ICompanyDirectory
{
    private readonly HashSet<Guid> _known;

    public FakeCompanyDirectory(params Guid[] knownCompanyIds)
    {
        _known = new HashSet<Guid>(knownCompanyIds);
    }

    public Task<CompanyRecord?> FindCompanyAsync(Guid companyId, CancellationToken ct = default) =>
        Task.FromResult(_known.Contains(companyId)
            ? new CompanyRecord(companyId, "Fake Co", "paid")
            : null);
}
