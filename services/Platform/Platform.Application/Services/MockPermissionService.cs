using Platform.Application.DTOs;
using Platform.Application.Interfaces;

namespace Platform.Application.Services;

/// <summary>
/// Mock implementation of IPermissionService for shell environments where the Platform API is unavailable.
/// Returns empty/null data to prevent hard failures in the UI.
/// </summary>
public class MockPermissionService : IPermissionService
{
    public Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        return Task.FromResult(Enumerable.Empty<PermissionDto>());
    }

    public Task<PermissionDto?> GetPermissionByIdAsync(Guid id)
    {
        return Task.FromResult<PermissionDto?>(null);
    }

    public Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createPermissionDto)
    {
        // Return a minimal permission DTO to prevent null reference exceptions
        return Task.FromResult(new PermissionDto
        {
            Id = Guid.NewGuid(),
            Name = createPermissionDto.Name,
            Description = createPermissionDto.Description,
            Module = createPermissionDto.Module,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task DeletePermissionAsync(Guid id)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<PermissionDto>> GetPermissionsByModuleAsync(string module)
    {
        return Task.FromResult(Enumerable.Empty<PermissionDto>());
    }
}
