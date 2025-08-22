using MPM_MVP.Models;
using MPM_MVP.DTOs;

namespace MPM_MVP.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDto dto);
    Task<User?> GetUserByIdAsync(int id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> UpdateUserAsync(int id, CreateUserDto dto);
    Task DeleteUserAsync(int id);
}