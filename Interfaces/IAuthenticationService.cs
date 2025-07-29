using BankRestApi.Models.DTOs;

namespace BankRestApi.Interfaces;

public interface IAuthenticationService
{
    Task<BaseResult<Token>> CreateAccessTokenAsync(string name, string password);

    Task<BaseResult<Token>> RefreshTokenAsync(Token token);

    Task RevokeRefreshToken(string refreshToken, string name);
}