using Microsoft.Extensions.AI;

namespace AI.Api.Services;

/// <summary>
/// Holds the per-tier <see cref="IChatClient"/> instances resolved at startup.
/// Either client may be null when its provider is not configured, allowing the
/// service to run demo-only or paid-only.
/// </summary>
public sealed class AiClientRegistry
{
    public AiClientRegistry(IChatClient? demoClient, IChatClient? paidClient)
    {
        DemoClient = demoClient;
        PaidClient = paidClient;
    }

    /// <summary>Free/demo tier client (GitHub Models), or null if not configured.</summary>
    public IChatClient? DemoClient { get; }

    /// <summary>Paid tier client (Bedrock/Claude), or null if not configured.</summary>
    public IChatClient? PaidClient { get; }

    /// <summary>Returns the client for the requested tier, if available.</summary>
    public IChatClient? For(AiTier tier) => tier == AiTier.Paid ? PaidClient : DemoClient;
}
