using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BankRestApi.Application.DTOs;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using User = BankRestApi.Domain.Entities.User;

namespace BankRestApi.Application.Services;

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
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        var refreshToken = Convert.ToBase64String(randomNumber);

        var securityKey = new SymmetricSecurityKey(
            Convert.FromBase64String(_config["Authentication:SecretForKey"]));
        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name)
        };

        var jwtSecurityToken = new JwtSecurityToken(
            _config["Authentication:Issuer"],
            _config["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _userRepository.Update(user, refreshToken, DateTime.UtcNow.AddHours(24));
        
        return new Token(accessToken, refreshToken);
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