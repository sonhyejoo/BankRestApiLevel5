using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services;

public class AuthenticationServiceTests
{
    private readonly FakePasswordHelper _passwordHelper;

    private readonly IUserRepository _userRepository;
    
    private readonly ITokenService _tokenService;
    
    public AuthenticationServiceTests()
    {
        _passwordHelper = new FakePasswordHelper();
        _userRepository = new FakeUserRepository();
        _tokenService = new FakeTokenService(_userRepository);
    }

    [Fact]
    public async Task CreateAccessTokenAsync_ValidLogin_ReturnsToken()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new LoginRequest("name", "password");
        await _userRepository.Add(new User{ Name = request.Name, HashedPassword = request.Password });

        var result = await authenticationService.CreateAccessTokenAsync(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.OK, new Token("accessToken", "refresh")));
    }
    
    [Fact]
    public async Task CreateAccessTokenAsync_InvalidName_ReturnsNotFound()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new LoginRequest("name", "password");
        await _userRepository.Add(new User{ Name = "invalid name", HashedPassword = request.Password });

        var result = await authenticationService.CreateAccessTokenAsync(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.NotFound, "Invalid name or password."));
    }
    
    [Fact]
    public async Task CreateAccessTokenAsync_WrongPassword_ReturnsNotFound()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new LoginRequest("name", "password");
        await _userRepository.Add(new User{ Name = request.Name, HashedPassword = "wrong password" });

        var result = await authenticationService.CreateAccessTokenAsync(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.NotFound, "Invalid name or password."));
    }
    
    
    [Fact]
    public async Task RefreshTokenAsync_ValidRequest_ReturnsToken()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new RefreshTokenRequest("name", "refresh");
        var user = await _userRepository.Add(new User{ Name = request.Name, HashedPassword = "password" });
        await _tokenService.BuildToken(user);

        var result = await authenticationService.RefreshTokenAsync(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.OK, new Token("accessToken", "refresh")));
    }
    
    [Fact]
    public async Task RefreshTokenAsync_InvalidName_ReturnsNotFound()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new RefreshTokenRequest("invalid name", "refresh");
        var user = await _userRepository.Add(new User{ Name = "name", HashedPassword = "password" });
        await _tokenService.BuildToken(user);

        var result = await authenticationService.RefreshTokenAsync(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.NotFound, "Please log in again."));
    }
    
    [Fact]
    public async Task RefreshTokenAsync_InvalidToken_ReturnsNotFound()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new RefreshTokenRequest("name", "invalid refresh token");
        var user = await _userRepository.Add(new User{ Name = "name", HashedPassword = "password" });
        await _tokenService.BuildToken(user);

        var result = await authenticationService.RefreshTokenAsync(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.NotFound, "Please log in again."));
    }

    
    [Fact]
    public async Task RevokeRefreshToken_ValidRequest_ReturnsNoContent()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new RevokeRequest("name", "refresh");
        var user = await _userRepository.Add(new User{ Name = "name", HashedPassword = "password" });
        await _tokenService.BuildToken(user);

        var result = await authenticationService.RevokeRefreshToken(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.NoContent, ""));
    }

    
    [Fact]
    public async Task RevokeRefreshToken_InvalidName_ReturnsNotFound()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new RevokeRequest("invalid name", "refresh");
        var user = await _userRepository.Add(new User{ Name = "name", HashedPassword = "password" });
        await _tokenService.BuildToken(user);

        var result = await authenticationService.RevokeRefreshToken(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.NotFound, "Invalid name or refresh token"));
    }
    
    [Fact]
    public async Task RevokeRefreshToken_InvalidToken_ReturnsNotFound()
    {
        var authenticationService = CreateDefaultAuthenticationService();
        var request = new RevokeRequest("name", "invalid token");
        var user = await _userRepository.Add(new User{ Name = "name", HashedPassword = "password" });
        await _tokenService.BuildToken(user);

        var result = await authenticationService.RevokeRefreshToken(request);
        
        Assert.Equivalent(
            result, 
            new BaseResult<Token>(HttpStatusCode.NotFound, "Invalid name or refresh token"));
    }


    private AuthenticationService CreateDefaultAuthenticationService() 
        => new (_passwordHelper,
            _tokenService,
            _userRepository);
}