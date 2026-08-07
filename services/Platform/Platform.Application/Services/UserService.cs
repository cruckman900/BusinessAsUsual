using Platform.Application.DTOs;
using Platform.Application.Interfaces;
using Platform.Domain.Entities;
using Platform.Domain.Interfaces;

namespace Platform.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Validate uniqueness
        if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
        {
            throw new InvalidOperationException($"Username '{createUserDto.Username}' already exists.");
        }

        if (await _userRepository.EmailExistsAsync(createUserDto.Email))
        {
            throw new InvalidOperationException($"Email '{createUserDto.Email}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            PhoneNumber = createUserDto.PhoneNumber,
            PasswordHash = PasswordHasher.HashPassword(createUserDto.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Assign roles
        foreach (var roleId in createUserDto.RoleIds)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });
        }

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        // Check email uniqueness if changed
        if (user.Email != updateUserDto.Email && await _userRepository.EmailExistsAsync(updateUserDto.Email))
        {
            throw new InvalidOperationException($"Email '{updateUserDto.Email}' already exists.");
        }

        user.Email = updateUserDto.Email;
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.PhoneNumber = updateUserDto.PhoneNumber;
        user.IsActive = updateUserDto.IsActive;

        // Update roles - clear and reassign
        user.UserRoles.Clear();
        foreach (var roleId in updateUserDto.RoleIds)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });
        }

        var updatedUser = await _userRepository.UpdateAsync(user);
        return MapToDto(updatedUser);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        if (!await _userRepository.ExistsAsync(id))
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        await _userRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(Guid roleId)
    {
        var users = await _userRepository.GetUsersByRoleAsync(roleId);
        return users.Select(MapToDto);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            ProfileImageUrl = user.ProfileImageUrl,
            Roles = user.UserRoles.Select(ur => new RoleDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name,
                Description = ur.Role.Description,
                IsSystemRole = ur.Role.IsSystemRole,
                CreatedAt = ur.Role.CreatedAt,
                UpdatedAt = ur.Role.UpdatedAt
            }).ToList()
        };
    }
}
