using System.Runtime.CompilerServices;
using System.Text;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Microsoft.Extensions.AI;

namespace AI.Api.Services.Providers;

/// <summary>
/// Adapts Amazon Bedrock's Converse API to the <see cref="IChatClient"/> abstraction so the
/// paid tier (Claude 3 Haiku) plugs into the same seam as every other provider.
/// AWS credentials are resolved through the standard AWS SDK credential chain.
/// </summary>
public sealed class BedrockChatClient : IChatClient
{
    private readonly IAmazonBedrockRuntime _runtime;
    private readonly string _modelId;

    public BedrockChatClient(string region, string modelId)
    {
        _runtime = new AmazonBedrockRuntimeClient(RegionEndpoint.GetBySystemName(region));
        _modelId = modelId;
    }

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);

        var request = BuildRequest(messages, options);
        var result = await _runtime.ConverseAsync(request, cancellationToken).ConfigureAwait(false);

        var text = ExtractText(result);
        var responseMessage = new ChatMessage(ChatRole.Assistant, text);

        return new ChatResponse(responseMessage)
        {
            ModelId = _modelId,
            FinishReason = MapFinishReason(result?.StopReason)
        };
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Non-streaming Converse is used and surfaced as a single update. Streaming can be
        // upgraded to ConverseStream later without changing callers.
        var response = await GetResponseAsync(messages, options, cancellationToken).ConfigureAwait(false);
        yield return new ChatResponseUpdate(ChatRole.Assistant, response.Text);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        return serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;
    }

    public void Dispose() => _runtime.Dispose();

    private ConverseRequest BuildRequest(IEnumerable<ChatMessage> messages, ChatOptions? options)
    {
        var systemBlocks = new List<SystemContentBlock>();
        var converseMessages = new List<Message>();

        foreach (var message in messages)
        {
            var text = message.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            if (message.Role == ChatRole.System)
            {
                systemBlocks.Add(new SystemContentBlock { Text = text });
                continue;
            }

            var role = message.Role == ChatRole.Assistant
                ? ConversationRole.Assistant
                : ConversationRole.User;

            // Bedrock requires alternating roles; merge consecutive same-role turns.
            if (converseMessages.Count > 0 && converseMessages[^1].Role == role)
            {
                converseMessages[^1].Content.Add(new ContentBlock { Text = text });
            }
            else
            {
                converseMessages.Add(new Message
                {
                    Role = role,
                    Content = new List<ContentBlock> { new() { Text = text } }
                });
            }
        }

        var request = new ConverseRequest
        {
            ModelId = _modelId,
            Messages = converseMessages,
            InferenceConfig = new InferenceConfiguration
            {
                MaxTokens = options?.MaxOutputTokens,
                Temperature = options?.Temperature
            }
        };

        if (systemBlocks.Count > 0)
        {
            request.System = systemBlocks;
        }

        return request;
    }

    private static string ExtractText(ConverseResponse? response)
    {
        var content = response?.Output?.Message?.Content;
        if (content is null || content.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var block in content)
        {
            if (!string.IsNullOrEmpty(block.Text))
            {
                sb.Append(block.Text);
            }
        }

        return sb.ToString();
    }

    private static ChatFinishReason? MapFinishReason(StopReason? stopReason)
    {
        if (stopReason is null)
        {
            return null;
        }

        if (stopReason == StopReason.Max_tokens)
        {
            return ChatFinishReason.Length;
        }

        if (stopReason == StopReason.Content_filtered)
        {
            return ChatFinishReason.ContentFilter;
        }

        return ChatFinishReason.Stop;
    }
}
