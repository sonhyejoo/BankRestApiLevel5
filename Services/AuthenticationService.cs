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
            return new BaseResult<Token>(HttpStatusCode.Unauthorized, "Account not found.");
        }

        if (!_passwordHelper.PasswordMatches(existingUser, password))
        {
            return new BaseResult<Token>(HttpStatusCode.Unauthorized, "Invalid name or password.");
        }

        var tokenToReturn = _tokenService.BuildToken(existingUser);
        
        return new BaseResult<Token>(HttpStatusCode.OK, tokenToReturn);
    }
    
    public async Task<BaseResult<Token>> RefreshTokenAsync(Token token)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(token.AccessToken);
        if (principal is null)
        {
            return new BaseResult<Token>(HttpStatusCode.Unauthorized, "Please log in again");
        }

        var name = principal.Identity?.Name;
        
        if (!await _tokenService.TakeRefreshToken(token.RefreshToken, name))
        {
            return new BaseResult<Token>(HttpStatusCode.BadRequest, "Please log in again.");
        }

        var user = await _userRepository.GetByName(name);

        var tokenToReturn = _tokenService.BuildToken(user);

        return new BaseResult<Token>(HttpStatusCode.OK, tokenToReturn);
    }

    public async Task RevokeRefreshToken(string refreshToken, string name)
    {
        await _tokenService.TakeRefreshToken(refreshToken, name);
    }
}