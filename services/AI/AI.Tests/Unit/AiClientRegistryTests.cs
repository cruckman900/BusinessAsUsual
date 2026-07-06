using AI.Api.Services;
using AI.Api.Services.Providers;
using AI.Tests.Fakes;
using Microsoft.Extensions.AI;

namespace AI.Tests.Unit;

/// <summary>Unit tests for <see cref="AiClientRegistry"/> tier selection.</summary>
public class AiClientRegistryTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public void For_ReturnsDemoClient_ForDemoTier()
    {
        var demo = new FakeChatClient();
        var paid = new FakeChatClient();
        var registry = new AiClientRegistry(demo, paid);

        Assert.Same(demo, registry.For(AiTier.Demo));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void For_ReturnsPaidClient_ForPaidTier()
    {
        var demo = new FakeChatClient();
        var paid = new FakeChatClient();
        var registry = new AiClientRegistry(demo, paid);

        Assert.Same(paid, registry.For(AiTier.Paid));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void For_ReturnsNull_WhenTierNotConfigured()
    {
        var registry = new AiClientRegistry(demoClient: null, paidClient: null);

        Assert.Null(registry.For(AiTier.Demo));
        Assert.Null(registry.For(AiTier.Paid));
    }
}

/// <summary>Unit tests for the development <see cref="StubChatClient"/>.</summary>
public class StubChatClientTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetResponseAsync_EchoesUserPrompt_WithStubModel()
    {
        var client = new StubChatClient();
        var messages = new[]
        {
            new ChatMessage(ChatRole.System, "system prompt"),
            new ChatMessage(ChatRole.User, "what is 2+2?")
        };

        var response = await client.GetResponseAsync(messages);

        Assert.Contains("PAID TIER STUB", response.Text);
        Assert.Contains("what is 2+2?", response.Text);
        Assert.Equal("stub-paid-model", response.ModelId);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetStreamingResponseAsync_YieldsAssistantUpdate()
    {
        var client = new StubChatClient();
        var messages = new[] { new ChatMessage(ChatRole.User, "hi") };

        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in client.GetStreamingResponseAsync(messages))
        {
            updates.Add(update);
        }

        Assert.Single(updates);
        Assert.Equal(ChatRole.Assistant, updates[0].Role);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void GetService_ReturnsSelf_ForAssignableType()
    {
        var client = new StubChatClient();

        Assert.Same(client, client.GetService(typeof(IChatClient)));
        Assert.Null(client.GetService(typeof(string)));
    }
}
