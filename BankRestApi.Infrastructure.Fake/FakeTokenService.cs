using System.Security.Cryptography;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Domain.Entities;

namespace BankRestApi.Infrastructure.Fake;

public class FakeTokenService : ITokenService
{
    private readonly IUserRepository _userRepository;

    public FakeTokenService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Token> BuildToken(User user)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        var refreshToken = Convert.ToBase64String(randomNumber);
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _userRepository.Update(user, refreshToken, DateTime.UtcNow.AddHours(24));

        return new Token("accessToken", refreshToken);
    }

    public async Task<User?> TakeRefreshToken(string name, string token)
    {
        var user = await _userRepository.Get(name);
        if (user is null
            || user.RefreshToken != token
            || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return null;
        }
        
        await _userRepository.Update(user, null, null);
            
        return user;
    }
}