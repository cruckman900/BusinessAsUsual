using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Entities;
using Platform.Domain.Interfaces;

namespace Platform.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();
        return permissions.Select(MapToDto);
    }

    public async Task<PermissionDto?> GetPermissionByIdAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        return permission != null ? MapToDto(permission) : null;
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createPermissionDto)
    {
        if (await _permissionRepository.NameExistsAsync(createPermissionDto.Name))
        {
            throw new InvalidOperationException($"Permission '{createPermissionDto.Name}' already exists.");
        }

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = createPermissionDto.Name,
            Description = createPermissionDto.Description,
            Module = createPermissionDto.Module,
            Resource = createPermissionDto.Resource,
            Action = createPermissionDto.Action,
            IsSystemPermission = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdPermission = await _permissionRepository.AddAsync(permission);
        return MapToDto(createdPermission);
    }

    public async Task DeletePermissionAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null)
        {
            throw new KeyNotFoundException($"Permission with ID {id} not found.");
        }

        if (permission.IsSystemPermission)
        {
            throw new InvalidOperationException("Cannot delete system permissions.");
        }

        await _permissionRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsByModuleAsync(string module)
    {
        var permissions = await _permissionRepository.GetPermissionsByModuleAsync(module);
        return permissions.Select(MapToDto);
    }

    private static PermissionDto MapToDto(Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            Module = permission.Module,
            Resource = permission.Resource,
            Action = permission.Action,
            IsSystemPermission = permission.IsSystemPermission,
            CreatedAt = permission.CreatedAt
        };
    }
}
