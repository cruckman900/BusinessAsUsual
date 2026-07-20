namespace AI.Api.Configuration;

/// <summary>
/// Strongly-typed binding for the "Ai" configuration section. Provider selection and
/// caps live here so models can be swapped by config rather than code changes.
/// </summary>
public sealed class AiOptions
{
    public const string SectionName = "Ai";

    /// <summary>System prompt prepended to every conversation.</summary>
    public string SystemPrompt { get; set; } = string.Empty;

    /// <summary>Maximum allowed characters in a user prompt (abuse/cost guard).</summary>
    public int MaxInputChars { get; set; } = 1000;

    /// <summary>Maximum tokens the model may generate per response (cost guard).</summary>
    public int MaxOutputTokens { get; set; } = 512;

    /// <summary>
    /// Maximum seconds to wait for the provider before failing fast with a clean
    /// "AI is busy" message instead of letting the request hang until the caller
    /// (web app) times out. Guards against a slow/unresponsive provider.
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 20;

    /// <summary>Free/demo tier used for the public test case (GitHub Models).</summary>
    public DemoTierOptions Demo { get; set; } = new();

    /// <summary>Paid tier reserved for provisioned companies (Claude Haiku via Bedrock).</summary>
    public PaidTierOptions Paid { get; set; } = new();

    /// <summary>Development-only helpers for testing paid-tier routing without a DB or AWS cost.</summary>
    public DevTierOptions Dev { get; set; } = new();
}

/// <summary>
/// Development-only options. These are ignored outside the Development environment and let
/// you exercise the paid tier without a real provisioned company or AWS Bedrock charges.
/// </summary>
public sealed class DevTierOptions
{
    /// <summary>
    /// When true (Development only), the paid tier uses an in-process stub client instead of
    /// Amazon Bedrock, so paid routing can be verified at zero AWS cost.
    /// </summary>
    public bool UseStubPaidClient { get; set; }

    /// <summary>
    /// Company ids (as strings) that the dev directory treats as provisioned, unlocking the
    /// paid tier without querying the provisioning database.
    /// </summary>
    public string[] TestCompanyIds { get; set; } = Array.Empty<string>();
}

/// <summary>Configuration for the free/demo tier (GitHub Models, OpenAI-compatible endpoint).</summary>
public sealed class DemoTierOptions
{
    public string Provider { get; set; } = "GitHubModels";
    public string Endpoint { get; set; } = "https://models.inference.ai.azure.com";
    public string Model { get; set; } = "gpt-4o-mini";

    /// <summary>
    /// GitHub token. Supplied via user-secrets ("Ai:Demo:ApiKey") in dev or the
    /// "Ai__Demo__ApiKey" environment variable in deploy. Never committed to source.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}

/// <summary>Configuration for the paid tier (Amazon Bedrock / Claude Haiku).</summary>
public sealed class PaidTierOptions
{
    public string Provider { get; set; } = "Bedrock";
    public string Region { get; set; } = "us-east-1";
    public string Model { get; set; } = "anthropic.claude-3-haiku-20240307-v1:0";
}
