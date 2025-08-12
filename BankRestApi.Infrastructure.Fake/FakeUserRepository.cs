using BankRestApi.Application.Interfaces;
using BankRestApi.Domain.Entities;

namespace BankRestApi.Infrastructure.Fake;

public class FakeUserRepository : IUserRepository
{
    private static readonly User? NullUser = null;
    
    private readonly Dictionary<string, User> _users;

    public FakeUserRepository()
    {
        _users = new Dictionary<string, User>();
    }
    
    public Task<User?> Add(User user)
    {
        var userName = user.Name;
        
        if (Contains(userName))
        {
            return Task.FromResult(NullUser);
        }

        _users.Add(user.Name, user);
        var clonedUser = user;

        return Task.FromResult(clonedUser);
    }

    public Task<User?> Get(string name)
    {
        if (!Contains(name))
        {
            return Task.FromResult(NullUser);
        }
        
        return Task.FromResult(_users[name]);
    }

    public Task Update(User user, string? refreshToken, DateTime? refreshTokenExpiry)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshTokenExpiry;
        var userName = user.Name;

        _users[userName] = user;

        var completedTask = Task.CompletedTask;
        return completedTask;
    }

    private bool Contains(string name) => _users.ContainsKey(name);
}