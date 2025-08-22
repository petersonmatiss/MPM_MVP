using Microsoft.EntityFrameworkCore;
using MPM_MVP.Models;
using MPM_MVP.DTOs;
using MPM_MVP.Interfaces;
using MPM_MVP.Data;

namespace MPM_MVP.Services;

public class UserService : IUserService
{
    private readonly MPMDbContext _context;
    
    public UserService(MPMDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
    
    public async Task<User> UpdateUserAsync(int id, CreateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new ArgumentException("User not found");
        
        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Role = dto.Role;
        
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}