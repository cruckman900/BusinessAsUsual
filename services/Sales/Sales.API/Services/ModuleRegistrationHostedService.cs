using Sales.Application.Services;

namespace Sales.API.Services;

public class ModuleRegistrationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ModuleRegistrationHostedService> _logger;

    public ModuleRegistrationHostedService(
        IServiceProvider serviceProvider,
        ILogger<ModuleRegistrationHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sales Module Registration starting...");

        // Small delay to let module registry start
        await Task.Delay(2000, cancellationToken);

        using var scope = _serviceProvider.CreateScope();
        var registrationService = scope.ServiceProvider.GetRequiredService<IModuleRegistrationService>();

        try
        {
            await registrationService.RegisterWithModuleRegistryAsync();
            _logger.LogInformation("Sales Module registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register Sales module");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sales Module Registration stopping...");
        return Task.CompletedTask;
    }
}
