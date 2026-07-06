using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModuleRegistry.Domain.Entities;
using ModuleRegistry.Domain.Repositories;

namespace ModuleRegistry.Tests.Functional;

/// <summary>
/// Boots the Module Registry API in-process for functional tests. Replaces the
/// EF-backed repository with an in-memory fake so the controller/service pipeline
/// can be exercised without a real database (the DbContext maps navigation items
/// as owned JSON, which the EF in-memory provider does not support).
/// </summary>
public class ModuleRegistryApiFactory : WebApplicationFactory<Program>
{
    public InMemoryModuleRepository Repository { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Production skips the dev-only migration/Swagger blocks in Program.cs.
        builder.UseEnvironment("Production");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IModuleRepository));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IModuleRepository>(Repository);
        });
    }

    /// <summary>
    /// Minimal in-memory <see cref="IModuleRepository"/> used only for functional tests.
    /// </summary>
    public sealed class InMemoryModuleRepository : IModuleRepository
    {
        private readonly List<ModuleMetadata> _modules = new();

        public Task<IEnumerable<ModuleMetadata>> GetAllAsync() =>
            Task.FromResult<IEnumerable<ModuleMetadata>>(_modules.ToList());

        public Task<ModuleMetadata?> GetByModuleIdAsync(string moduleId) =>
            Task.FromResult(_modules.FirstOrDefault(m => m.ModuleId == moduleId));

        public Task<IEnumerable<ModuleMetadata>> GetActiveAsync() =>
            Task.FromResult<IEnumerable<ModuleMetadata>>(_modules.Where(m => m.IsActive).ToList());

        public Task<IEnumerable<ModuleMetadata>> GetModulesWithUiAsync() =>
            Task.FromResult<IEnumerable<ModuleMetadata>>(
                _modules.Where(m => m.IsActive && m.UiEntryPoint != null).ToList());

        public Task<IEnumerable<ModuleMetadata>> GetModulesWithMobileAsync() =>
            Task.FromResult<IEnumerable<ModuleMetadata>>(
                _modules.Where(m => m.IsActive && m.SupportsMobile && m.MobileUISpecUrl != null).ToList());

        public Task AddOrUpdateAsync(ModuleMetadata module)
        {
            _modules.RemoveAll(m => m.ModuleId == module.ModuleId);
            _modules.Add(module);
            return Task.CompletedTask;
        }

        public Task UpdateHealthStatusAsync(string moduleId, string status, DateTime lastChecked)
        {
            var module = _modules.FirstOrDefault(m => m.ModuleId == moduleId);
            if (module != null)
            {
                module.HealthStatus = status;
                module.LastHealthCheck = lastChecked;
            }
            return Task.CompletedTask;
        }
    }
}
