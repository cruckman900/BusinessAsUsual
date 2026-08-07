using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Domain.Interfaces;
using Platform.Infrastructure.Data;
using Platform.Infrastructure.Repositories;

namespace Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<PlatformDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PlatformDb") ?? 
                "Server=(localdb)\\mssqllocaldb;Database=BusinessAsUsual_Platform;Trusted_Connection=True;MultipleActiveResultSets=true"));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        return services;
    }
}
