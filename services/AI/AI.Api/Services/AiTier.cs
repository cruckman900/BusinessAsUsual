namespace AI.Api.Services;

/// <summary>Identifies which AI service tier a request should be routed to.</summary>
public enum AiTier
{
    /// <summary>Free/demo tier (GitHub Models) for the public test case.</summary>
    Demo,

    /// <summary>Paid tier (Claude Haiku via Bedrock) for provisioned companies.</summary>
    Paid
}
