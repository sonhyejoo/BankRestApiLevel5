using BankRestApi.Models.DTOs;

namespace BankRestApi.Interfaces;

public interface IAuthenticationService
{
    Task<BaseResult<Token>> CreateAccessTokenAsync(string name, string password);

    Task<BaseResult<Token>> RefreshTokenAsync(string name, string refreshToken);

    Task<BaseResult<Token>> RevokeRefreshToken(string refreshToken);
}