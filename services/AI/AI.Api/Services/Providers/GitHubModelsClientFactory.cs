using System.ClientModel;
using AI.Api.Configuration;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AI.Api.Services.Providers;

/// <summary>
/// Builds an <see cref="IChatClient"/> for the free/demo tier backed by GitHub Models.
/// GitHub Models exposes an OpenAI-compatible endpoint, so the OpenAI SDK is pointed at
/// the GitHub Models base address and authenticated with a GitHub token.
/// </summary>
public static class GitHubModelsClientFactory
{
    /// <summary>
    /// Creates the demo <see cref="IChatClient"/>, or null when no token is configured
    /// (so the app can still start for paid-only or offline scenarios).
    /// </summary>
    public static IChatClient? TryCreate(DemoTierOptions options, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            logger.LogWarning(
                "Demo tier (GitHub Models) has no ApiKey configured. Set 'Ai:Demo:ApiKey' via user-secrets or the 'Ai__Demo__ApiKey' environment variable to enable it.");
            return null;
        }

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(options.Endpoint)
        };

        var chatClient = new OpenAI.Chat.ChatClient(
            options.Model,
            new ApiKeyCredential(options.ApiKey),
            clientOptions);

        logger.LogInformation(
            "Demo tier (GitHub Models) is configured. Endpoint={Endpoint}, Model={Model}.",
            options.Endpoint, options.Model);

        return chatClient.AsIChatClient();
    }
}
