using AI.Api.Services;
using AI.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Tests.Functional;

/// <summary>
/// Boots the AI API in-process for functional tests. Replaces the real
/// <see cref="AiClientRegistry"/> and <see cref="ICompanyDirectory"/> with fakes so no
/// GitHub Models / Bedrock / SQL dependencies are needed. Runs in Production to skip the
/// dev-only stub wiring.
/// </summary>
public class AiApiFactory : WebApplicationFactory<Program>
{
    // xUnit's IClassFixture<T> requires a parameterless constructor, so the provisioned
    // company id is a fixed value created once per fixture instance rather than injected.
    private readonly Guid _provisionedCompanyId = Guid.NewGuid();

    public Guid ProvisionedCompanyId => _provisionedCompanyId;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");

        builder.ConfigureServices(services =>
        {
            RemoveAll<AiClientRegistry>(services);
            RemoveAll<ICompanyDirectory>(services);

            var demo = new FakeChatClient("demo answer", "demo-model");
            var paid = new FakeChatClient("paid answer", "paid-model");
            services.AddSingleton(new AiClientRegistry(demo, paid));
            services.AddSingleton<ICompanyDirectory>(new FakeCompanyDirectory(_provisionedCompanyId));
        });
    }

    private static void RemoveAll<T>(IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(T)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
}
