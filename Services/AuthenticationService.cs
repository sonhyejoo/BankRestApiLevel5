using System.Net;
using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;
using Microsoft.Identity.Client;

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
    
    public async Task<BaseResult<Token>> CreateAccessTokenAsync(string name, string password)
    {
        var existingUser = await _userRepository.GetByName(name);
        if (existingUser is null)
        {
            return new BaseResult<Token>(HttpStatusCode.Unauthorized, "Invalid name or password.");
        }

        if (!_passwordHelper.PasswordMatches(existingUser, password))
        {
            return new BaseResult<Token>(HttpStatusCode.Unauthorized, "Invalid name or password.");
        }

        var tokenToReturn = await _tokenService.BuildToken(existingUser);
        
        return new BaseResult<Token>(HttpStatusCode.OK, tokenToReturn);
    }
    
    public async Task<BaseResult<Token>> RefreshTokenAsync(string name, string refreshToken)
    {
        var user = await _tokenService.TakeRefreshToken(name, refreshToken);
        if (user is null)
        {
            return new BaseResult<Token>(HttpStatusCode.BadRequest, "Please log in again.");
        }
        
        var tokenToReturn = await _tokenService.BuildToken(user);

        return new BaseResult<Token>(HttpStatusCode.OK, tokenToReturn);
    }

    public async Task<BaseResult<Token>> RevokeRefreshToken(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshToken(refreshToken);
        if (user is null)
        {
            return new BaseResult<Token>(HttpStatusCode.BadRequest, "Invalid refresh token");
        }

        await _userRepository.Update(user, null, null);

        return new BaseResult<Token>(HttpStatusCode.NoContent, "");
    }
}