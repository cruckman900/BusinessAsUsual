using Platform.Application.DTOs;
using Platform.Application.Interfaces;

namespace Platform.Application.Services;

/// <summary>
/// Mock implementation of IRoleService for shell environments where the Platform API is unavailable.
/// Returns empty/null data to prevent hard failures in the UI.
/// </summary>
public class MockRoleService : IRoleService
{
    public Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        return Task.FromResult(Enumerable.Empty<RoleDto>());
    }

    public Task<RoleDto?> GetRoleByIdAsync(Guid id)
    {
        return Task.FromResult<RoleDto?>(null);
    }

    public Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        // Return a minimal role DTO to prevent null reference exceptions
        return Task.FromResult(new RoleDto
        {
            Id = Guid.NewGuid(),
            Name = createRoleDto.Name,
            Description = createRoleDto.Description,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto)
    {
        // Return a minimal role DTO to prevent null reference exceptions
        return Task.FromResult(new RoleDto
        {
            Id = id,
            Name = "Mock Role",
            Description = "Mock role description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public Task DeleteRoleAsync(Guid id)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<RoleDto>> GetRolesByUserAsync(Guid userId)
    {
        return Task.FromResult(Enumerable.Empty<RoleDto>());
    }
}
