using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BankRestApi.Interfaces;
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

    public async Task<string?> TakeRefreshToken(string refreshToken, string name)
    {
        var user = await _userRepository.GetByName(name);
        if (user is null
            || user.RefreshToken != refreshToken
            || user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            return null;
        }

        var newRefreshToken = BuildRefreshToken();
        var newExpiryTime = DateTime.UtcNow.AddHours(24);

        await _userRepository.Update(user, newRefreshToken, newExpiryTime);

        return newRefreshToken;
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
            new Claim("name", user.AccountName)
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