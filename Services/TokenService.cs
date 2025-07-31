using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using User = BankRestApi.Models.User;

namespace BankRestApi.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    
    private readonly IUserRepository _userRepository;

    public TokenService(IConfiguration config, IUserRepository userRepository)
    {
        _config = config;
        _userRepository = userRepository;
    }
    
    public async Task<Token> BuildToken(User user)
    {
        var accessToken = BuildAccessToken(user);
        var refreshToken = BuildRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _userRepository.Update(user, refreshToken, DateTime.UtcNow.AddHours(24));
        
        return new Token(accessToken, refreshToken);
    }

    public async Task<User?> TakeRefreshToken(string name, string token)
    {
        var user = await _userRepository.GetByName(name);
        if (user is null
            || user.RefreshToken != token
            || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            Console.WriteLine($"{user is null}");
            return null;
        }
        
        await _userRepository.Update(user, null, null);
            
        return user;
    }

    public string BuildRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber);
    }

    public string BuildAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Convert.FromBase64String(_config["Authentication:SecretForKey"]));
        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name)
        };

        var jwtSecurityToken = new JwtSecurityToken(
            _config["Authentication:Issuer"],
            _config["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}