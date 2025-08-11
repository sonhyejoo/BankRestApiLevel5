using BankRestApi.Application.Interfaces;
using BankRestApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankRestApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Add(User user)
    {
        var result = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public Task<User?> Get(string name)
        => _context.Users.FirstOrDefaultAsync(u => u.Name == name);
    
    public async Task Update(User user, string? refreshToken, DateTime? refreshTokenExpiry)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshTokenExpiry;
        
        await _context.SaveChangesAsync();
    }
}