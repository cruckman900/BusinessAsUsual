using AI.Api.Configuration;
using AI.Api.Services;
using AI.Tests.Fakes;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AI.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="AiChatService"/> covering guardrails, tier resolution,
/// provider fallback, prompt assembly, and error handling.
/// </summary>
public class AiChatServiceTests
{
    private static AiOptions DefaultOptions() => new()
    {
        SystemPrompt = "You are a helpful assistant.",
        MaxInputChars = 100,
        MaxOutputTokens = 256,
        RequestTimeoutSeconds = 20
    };

    private static AiChatService CreateService(
        AiOptions options,
        IChatClient? demoClient,
        IChatClient? paidClient,
        ICompanyDirectory directory)
    {
        var registry = new AiClientRegistry(demoClient, paidClient);
        return new AiChatService(
            registry,
            directory,
            Options.Create(options),
            NullLogger<AiChatService>.Instance);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_ReturnsPrompt_WhenQueryEmpty()
    {
        var service = CreateService(DefaultOptions(), new FakeChatClient(), null, new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest("   ", null));

        Assert.Equal("Please enter a question.", result.Answer);
        Assert.Equal(nameof(AiTier.Demo), result.Tier);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_ReturnsTooLong_WhenQueryExceedsCap()
    {
        var options = DefaultOptions();
        var service = CreateService(options, new FakeChatClient(), null, new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest(new string('x', options.MaxInputChars + 1), null));

        Assert.Contains("too long", result.Answer);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_UsesDemoTier_WhenNoCompanyId()
    {
        var demo = new FakeChatClient("demo response", "demo-model");
        var service = CreateService(DefaultOptions(), demo, new FakeChatClient("paid", "paid-model"), new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest("hello", null));

        Assert.Equal("demo response", result.Answer);
        Assert.Equal(nameof(AiTier.Demo), result.Tier);
        Assert.Equal("demo-model", result.Model);
        Assert.Equal(1, demo.CallCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_UsesDemoTier_WhenCompanyIdUnknown()
    {
        var demo = new FakeChatClient("demo response");
        var paid = new FakeChatClient("paid response");
        var service = CreateService(DefaultOptions(), demo, paid, new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest("hello", Guid.NewGuid()));

        Assert.Equal(nameof(AiTier.Demo), result.Tier);
        Assert.Equal(0, paid.CallCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_UsesPaidTier_WhenCompanyIdProvisioned()
    {
        var companyId = Guid.NewGuid();
        var demo = new FakeChatClient("demo response");
        var paid = new FakeChatClient("paid response", "paid-model");
        var service = CreateService(DefaultOptions(), demo, paid, new FakeCompanyDirectory(companyId));

        var result = await service.AskAsync(new AiChatRequest("hello", companyId));

        Assert.Equal("paid response", result.Answer);
        Assert.Equal(nameof(AiTier.Paid), result.Tier);
        Assert.Equal("paid-model", result.Model);
        Assert.Equal(1, paid.CallCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_FallsBackToDemo_WhenPaidNotConfigured()
    {
        var companyId = Guid.NewGuid();
        var demo = new FakeChatClient("demo response");
        // Paid client is null even though the company is provisioned.
        var service = CreateService(DefaultOptions(), demo, null, new FakeCompanyDirectory(companyId));

        var result = await service.AskAsync(new AiChatRequest("hello", companyId));

        Assert.Equal("demo response", result.Answer);
        Assert.Equal(nameof(AiTier.Demo), result.Tier);
        Assert.Equal(1, demo.CallCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_ReturnsNotConfigured_WhenNoClientsAvailable()
    {
        var service = CreateService(DefaultOptions(), demoClient: null, paidClient: null, new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest("hello", null));

        Assert.Contains("not configured", result.Answer);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_InjectsSystemPromptAndOutputCap()
    {
        var options = DefaultOptions();
        var demo = new FakeChatClient();
        var service = CreateService(options, demo, null, new FakeCompanyDirectory());

        await service.AskAsync(new AiChatRequest("hello", null));

        Assert.NotNull(demo.LastMessages);
        Assert.Equal(ChatRole.System, demo.LastMessages![0].Role);
        Assert.Equal(options.SystemPrompt, demo.LastMessages[0].Text);
        Assert.Equal(ChatRole.User, demo.LastMessages[1].Role);
        Assert.Equal(options.MaxOutputTokens, demo.LastOptions!.MaxOutputTokens);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_OmitsSystemMessage_WhenPromptEmpty()
    {
        var options = DefaultOptions();
        options.SystemPrompt = string.Empty;
        var demo = new FakeChatClient();
        var service = CreateService(options, demo, null, new FakeCompanyDirectory());

        await service.AskAsync(new AiChatRequest("hello", null));

        Assert.Single(demo.LastMessages!);
        Assert.Equal(ChatRole.User, demo.LastMessages![0].Role);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_ReturnsFriendlyError_WhenProviderThrows()
    {
        var demo = new FakeChatClient { ThrowOnCall = true };
        var service = CreateService(DefaultOptions(), demo, null, new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest("hello", null));

        Assert.Contains("Something went wrong", result.Answer);
        Assert.Null(result.Model);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_ReturnsBusyMessage_WhenProviderTimesOut()
    {
        var options = DefaultOptions();
        options.RequestTimeoutSeconds = 1;
        var demo = new FakeChatClient { Delay = TimeSpan.FromSeconds(5) };
        var service = CreateService(options, demo, null, new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest("hello", null));

        Assert.Contains("taking longer than usual", result.Answer);
        Assert.Null(result.Model);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_ReturnsFallbackText_WhenProviderReturnsEmpty()
    {
        var demo = new FakeChatClient(responseText: string.Empty);
        var service = CreateService(DefaultOptions(), demo, null, new FakeCompanyDirectory());

        var result = await service.AskAsync(new AiChatRequest("hello", null));

        Assert.Contains("couldn't generate a response", result.Answer);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskAsync_TreatsEmptyGuid_AsDemoTier()
    {
        var paid = new FakeChatClient("paid response");
        var demo = new FakeChatClient("demo response");
        var service = CreateService(DefaultOptions(), demo, paid, new FakeCompanyDirectory(Guid.Empty));

        var result = await service.AskAsync(new AiChatRequest("hello", Guid.Empty));

        Assert.Equal(nameof(AiTier.Demo), result.Tier);
        Assert.Equal(0, paid.CallCount);
    }
}
