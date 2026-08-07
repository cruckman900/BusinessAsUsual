using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Entities;
using Platform.Domain.Interfaces;

namespace Platform.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(MapToDto);
    }

    public async Task<RoleDto?> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return role != null ? MapToDto(role) : null;
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        if (await _roleRepository.NameExistsAsync(createRoleDto.Name))
        {
            throw new InvalidOperationException($"Role '{createRoleDto.Name}' already exists.");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = createRoleDto.Name,
            Description = createRoleDto.Description,
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow
        };

        // Assign permissions
        foreach (var permissionId in createRoleDto.PermissionIds)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId,
                AssignedAt = DateTime.UtcNow
            });
        }

        var createdRole = await _roleRepository.AddAsync(role);
        return MapToDto(createdRole);
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {id} not found.");
        }

        if (role.IsSystemRole)
        {
            throw new InvalidOperationException("Cannot modify system roles.");
        }

        // Check name uniqueness if changed
        if (role.Name != updateRoleDto.Name && await _roleRepository.NameExistsAsync(updateRoleDto.Name))
        {
            throw new InvalidOperationException($"Role '{updateRoleDto.Name}' already exists.");
        }

        role.Name = updateRoleDto.Name;
        role.Description = updateRoleDto.Description;

        // Update permissions - clear and reassign
        role.RolePermissions.Clear();
        foreach (var permissionId in updateRoleDto.PermissionIds)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId,
                AssignedAt = DateTime.UtcNow
            });
        }

        var updatedRole = await _roleRepository.UpdateAsync(role);
        return MapToDto(updatedRole);
    }

    public async Task DeleteRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {id} not found.");
        }

        if (role.IsSystemRole)
        {
            throw new InvalidOperationException("Cannot delete system roles.");
        }

        await _roleRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesByUserAsync(Guid userId)
    {
        var roles = await _roleRepository.GetRolesByUserAsync(userId);
        return roles.Select(MapToDto);
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            Permissions = role.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description,
                Module = rp.Permission.Module,
                Resource = rp.Permission.Resource,
                Action = rp.Permission.Action,
                IsSystemPermission = rp.Permission.IsSystemPermission,
                CreatedAt = rp.Permission.CreatedAt
            }).ToList()
        };
    }
}
