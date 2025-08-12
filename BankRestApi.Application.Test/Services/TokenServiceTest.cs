using System.IdentityModel.Tokens.Jwt;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BankRestApi.Application.Test.Services.TokenServiceTests;

public class TokenServiceTest
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;

    public TokenServiceTest()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Authentication:SecretForKey", "whg2k0wRcd9SswTahmgr45jydkM7vJNXWcElS6LsFMQ="},
            {"Authentication:Issuer", "https://localhost:7125"},
            {"Authentication:Audience", "bankrestapi"}
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        _userRepository = new FakeUserRepository();
    }

    [Fact]
    public async Task BuildToken_ValidUser_ReturnsValidToken()
    {
        var tokenService = CreateDefaultTokenService();
        var user = new User { Name = "name", HashedPassword = "password" };

        var (token, _) = await tokenService.BuildToken(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();
        var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);
        
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task BuildToken_ValidUser_RefreshTokenSavedToUser()
    {
        var tokenService = CreateDefaultTokenService();
        var user = new User { Name = "name", HashedPassword = "password" };
        await _userRepository.Add(user);

        var (_, refreshToken) = await tokenService.BuildToken(user);

        var userFromRepository = await _userRepository.Get(user.Name);
        
        Assert.True(refreshToken == userFromRepository.RefreshToken);
    }
    
    [Fact]
    public async Task TakeRefreshToken_ValidName_RefreshTokenRemovedFromUser()
    {
        var tokenService = CreateDefaultTokenService();
        var user = new User { Name = "name", HashedPassword = "password" };
        var addedUser = await _userRepository.Add(user);
        var (_, refreshToken) = await tokenService.BuildToken(addedUser);
        
        var resultUser = await tokenService.TakeRefreshToken(addedUser.Name, refreshToken);

        Assert.NotNull(resultUser);
        Assert.Null(resultUser.RefreshToken);
        Assert.Null(resultUser.RefreshTokenExpiry);
    }
    
    [Fact]
    public async Task TakeRefreshToken_InvalidName_ReturnsNull()
    {
        var tokenService = CreateDefaultTokenService();
        var user = new User { Name = "name", HashedPassword = "password" };
        var addedUser = await _userRepository.Add(user);
        var (_, refreshToken) = await tokenService.BuildToken(addedUser);
        
        var resultUser = await tokenService.TakeRefreshToken("not name", refreshToken);
        
        Assert.Null(resultUser);
        Assert.NotNull(addedUser.RefreshToken);
    }

    [Fact]
    public async Task TakeRefreshToken_InvalidRefreshToken_ReturnsNull()
    {
        var tokenService = CreateDefaultTokenService();
        var user = new User { Name = "name", HashedPassword = "password" };
        var addedUser = await _userRepository.Add(user);
        var (_, refreshToken) = await tokenService.BuildToken(addedUser);
        
        var resultUser = await tokenService.TakeRefreshToken("name", "wrong refresh token");
        
        Assert.Null(resultUser);
        Assert.NotNull(addedUser.RefreshToken);
    }
    
    [Fact]
    public async Task TakeRefreshToken_ExpiredToken_ReturnsNull()
    {
        var tokenService = CreateDefaultTokenService();
        var user = new User { Name = "name", HashedPassword = "password" };
        var addedUser = await _userRepository.Add(user);
        var (_, refreshToken) = await tokenService.BuildToken(addedUser);
        addedUser.RefreshTokenExpiry = DateTime.UtcNow;
        
        var resultUser = await tokenService.TakeRefreshToken(addedUser.Name, refreshToken);
        
        Assert.Null(resultUser);
    }
    
    private TokenService CreateDefaultTokenService() => new(_config, _userRepository);
    private TokenValidationParameters GetValidationParameters() => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _config["Authentication:Issuer"],
        ValidAudience = _config["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Convert.FromBase64String(_config["Authentication:SecretForKey"]))
    };
}