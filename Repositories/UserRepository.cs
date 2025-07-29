using BankRestApi.Interfaces;
using BankRestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankRestApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Insert(User user)
    {
        var result = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public Task<User?> GetByName(string name)
        => _context.Users.FirstOrDefaultAsync(u => u.AccountName == name);

    public async Task<User?> Update(User user, string? refreshToken, DateTime? refreshTokenExpiry)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshTokenExpiry;
        await _context.SaveChangesAsync();

        return user;
    }
}