using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;

namespace BankRestApi.Interfaces;

public interface IAuthenticationService
{
    Task<BaseResult<Token>> CreateAccessTokenAsync(LoginRequest request);

    Task<BaseResult<Token>> RefreshTokenAsync(RefreshTokenRequest request);

    Task<BaseResult<Token>> RevokeRefreshToken(RevokeRequest request);
}