using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IUserRepository
{
    Task<User?> Insert(User user);

    Task<User?> GetByName(string name);

    Task<User?> GetByRefreshToken(string refreshToken);

    Task<User?> Update(User user, string? refreshToken, DateTime? refreshTokenExpiry);
}