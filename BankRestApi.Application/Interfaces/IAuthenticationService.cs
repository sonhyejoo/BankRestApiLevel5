using BankRestApi.Application.DTOs;
using BankRestApi.Application.DTOs.Authentication;

namespace BankRestApi.Application.Interfaces;

public interface IAuthenticationService
{
    Task<BaseResult<Token>> CreateAccessTokenAsync(LoginRequest request);

    Task<BaseResult<Token>> RefreshTokenAsync(RefreshTokenRequest request);

    Task<BaseResult<Token>> RevokeRefreshToken(RevokeRequest request);
}