using System.Security.Claims;
using BankRestApi.Models.DTOs;
using User = BankRestApi.Models.User;

namespace BankRestApi.Interfaces;

public interface ITokenService
{
    Token BuildToken(User user);
    
    Task<bool> TakeRefreshToken(string token, string name);
    
    string BuildRefreshToken();

    string BuildAccessToken(User user);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
}