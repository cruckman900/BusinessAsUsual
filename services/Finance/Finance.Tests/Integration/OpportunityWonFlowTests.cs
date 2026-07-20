using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Finance.Application.Events;
using Finance.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Finance.Tests.Integration;

/// <summary>
/// Proves the in-process event bus flow: publishing OpportunityWon causes the
/// Finance handler to create a linked draft invoice.
/// </summary>
public class OpportunityWonFlowTests
{
    [Trait("Category", "Integration")]
    [Fact]
    public async Task PublishingOpportunityWon_CreatesLinkedDraftInvoice()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<FinanceDataStore>();
        services.AddScoped<IInvoiceService, MockInvoiceService>();
        services.AddInProcessEventBus();
        services.AddIntegrationEventHandler<OpportunityWonIntegrationEvent, OpportunityWonHandler>();

        using var provider = services.BuildServiceProvider();

        // Start the dispatcher background service.
        var dispatcher = provider.GetServices<IHostedService>().OfType<EventBusDispatcher>().Single();
        await dispatcher.StartAsync(CancellationToken.None);

        var bus = provider.GetRequiredService<IEventBus>();
        await bus.PublishAsync(new OpportunityWonIntegrationEvent
        {
            OpportunityId = "OPP-123",
            OpportunityName = "Big Deal",
            CustomerName = "Acme",
            Amount = 5000m,
            Quantity = 2
        });

        // Wait for the async handler to run.
        var store = provider.GetRequiredService<FinanceDataStore>();
        var created = await WaitForAsync(
            () => store.Invoices.FirstOrDefault(i => i.SourceReferenceId == "OPP-123"),
            TimeSpan.FromSeconds(5));

        await dispatcher.StopAsync(CancellationToken.None);

        Assert.NotNull(created);
        Assert.Equal("crm", created!.SourceModule);
        Assert.Equal(Finance.Domain.Enums.InvoiceStatus.Draft, created.Status);
        Assert.Equal(5000m, created.Total);
    }

    private static async Task<T?> WaitForAsync<T>(Func<T?> probe, TimeSpan timeout) where T : class
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var result = probe();
            if (result is not null) return result;
            await Task.Delay(50);
        }
        return probe();
    }
}
