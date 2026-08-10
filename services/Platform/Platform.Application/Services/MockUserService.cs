using Platform.Application.DTOs;
using Platform.Application.Interfaces;

namespace Platform.Application.Services;

/// <summary>
/// Mock implementation of IUserService for shell environments where the Platform API is unavailable.
/// Returns empty/null data to prevent hard failures in the UI.
/// </summary>
public class MockUserService : IUserService
{
    public Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return Task.FromResult(Enumerable.Empty<UserDto>());
    }

    public Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        return Task.FromResult<UserDto?>(null);
    }

    public Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        return Task.FromResult<UserDto?>(null);
    }

    public Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Return a minimal user DTO to prevent null reference exceptions
        return Task.FromResult(new UserDto
        {
            Id = Guid.NewGuid(),
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            FullName = $"{createUserDto.FirstName} {createUserDto.LastName}",
            PhoneNumber = createUserDto.PhoneNumber,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        // Return a minimal user DTO to prevent null reference exceptions
        return Task.FromResult(new UserDto
        {
            Id = id,
            Username = "mock_user",
            Email = "mock@example.com",
            FirstName = "Mock",
            LastName = "User",
            FullName = "Mock User",
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public Task DeleteUserAsync(Guid id)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<UserDto>> GetUsersByRoleAsync(Guid roleId)
    {
        return Task.FromResult(Enumerable.Empty<UserDto>());
    }
}
