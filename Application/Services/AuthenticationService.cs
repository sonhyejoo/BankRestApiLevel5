using System.Net;
using Application.DTOs;
using BankRestApi.Application.DTOs.Authentication;
using BankRestApi.Interfaces;

namespace BankRestApi.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHelper _passwordHelper;
    private readonly ITokenService _tokenService;

    public AuthenticationService(
        IPasswordHelper passwordHelper,
        ITokenService tokenService,
        IUserRepository userRepository)
    {
        _passwordHelper = passwordHelper;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<BaseResult<Token>> CreateAccessTokenAsync(LoginRequest request)
    {
        var (name, password) = request;
        var existingUser = await _userRepository.Get(name);
        if (existingUser is null || !_passwordHelper.PasswordMatches(existingUser, password))
        {
            return new BaseResult<Token>(HttpStatusCode.NotFound, "Invalid name or password.");
        }

        var tokenToReturn = await _tokenService.BuildToken(existingUser);

        return new BaseResult<Token>(HttpStatusCode.OK, tokenToReturn);
    }
    
    public async Task<BaseResult<Token>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var (name, refreshToken) = request;
        var user = await _tokenService.TakeRefreshToken(name, refreshToken);
        if (user is null)
        {
            return new BaseResult<Token>(HttpStatusCode.NotFound, "Please log in again.");
        }

        var tokenToReturn = await _tokenService.BuildToken(user);

        return new BaseResult<Token>(HttpStatusCode.OK, tokenToReturn);
    }

    public async Task<BaseResult<Token>> RevokeRefreshToken(RevokeRequest request)
    {
        var (name, refreshToken) = request;
        var user = await _userRepository.Get(name);
        if (user is null || user.RefreshToken != refreshToken)
        {
            return new BaseResult<Token>(HttpStatusCode.NotFound, "Invalid name or refresh token");
        }
        
        await _userRepository.Update(user, null, null);

        return new BaseResult<Token>(HttpStatusCode.NoContent, "");
    }
}