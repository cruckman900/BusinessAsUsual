using HR.Application.Services;

namespace HR.API.Services;

/// <summary>
/// Keeps the HR module registered with the Module Registry.
/// Retries on startup until the registry is reachable, then re-registers
/// periodically so the module survives a registry restart (the registry uses
/// an in-memory store in development and forgets registrations when it restarts).
/// </summary>
public class ModuleRegistrationHostedService : BackgroundService
{
    private static readonly TimeSpan StartupRetryDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(30);

    private readonly IServiceProvider _services;
    private readonly ILogger<ModuleRegistrationHostedService> _logger;

    public ModuleRegistrationHostedService(IServiceProvider services, ILogger<ModuleRegistrationHostedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initial registration: retry until the registry accepts us.
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await TryRegisterAsync(stoppingToken))
                break;

            await SafeDelay(StartupRetryDelay, stoppingToken);
        }

        // Heartbeat: re-register periodically to survive registry restarts.
        while (!stoppingToken.IsCancellationRequested)
        {
            await SafeDelay(HeartbeatInterval, stoppingToken);
            if (stoppingToken.IsCancellationRequested) break;
            await TryRegisterAsync(stoppingToken);
        }
    }

    private async Task<bool> TryRegisterAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _services.CreateScope();
            var registration = scope.ServiceProvider.GetRequiredService<IModuleRegistrationService>();
            await registration.RegisterWithModuleRegistryAsync();
            return true;
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("Module registration attempt failed: {Message}", ex.Message);
            return false;
        }
    }

    private static async Task SafeDelay(TimeSpan delay, CancellationToken token)
    {
        try { await Task.Delay(delay, token); }
        catch (TaskCanceledException) { /* shutting down */ }
    }
}
