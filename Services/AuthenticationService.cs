using System.Net;
using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;

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
        var existingUser = await _userRepository.GetByName(name);
        if (existingUser is null || !_passwordHelper.PasswordMatches(existingUser, password))
        {
            return new BaseResult<Token>(HttpStatusCode.Unauthorized, "Invalid name or password.");
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
            return new BaseResult<Token>(HttpStatusCode.BadRequest, "Please log in again.");
        }

        var tokenToReturn = await _tokenService.BuildToken(user);

        return new BaseResult<Token>(HttpStatusCode.OK, tokenToReturn);
    }

    public async Task<BaseResult<Token>> RevokeRefreshToken(RevokeRequest request)
    {
        var (name, refreshToken) = request;
        var user = await _userRepository.GetByName(name);
        if (user is null || user.RefreshToken != refreshToken)
        {
            return new BaseResult<Token>(HttpStatusCode.BadRequest, "Invalid name or refresh token");
        }
        
        await _userRepository.Update(user, null, null);

        return new BaseResult<Token>(HttpStatusCode.NoContent, "");
    }
}