using Platform.Application.DTOs;

namespace Platform.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
    Task DeleteUserAsync(Guid id);
    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(Guid roleId);
}
