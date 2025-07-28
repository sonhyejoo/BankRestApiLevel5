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
    public async Task Insert(User user)
        => await _context.Users.AddAsync(user);

    public async Task<User?> GetByName(string name)
        => await _context.Users.FirstOrDefaultAsync(u => u.AccountName == name);
}