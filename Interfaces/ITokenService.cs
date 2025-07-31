using System.Security.Claims;
using BankRestApi.Models.DTOs;
using User = BankRestApi.Models.User;

namespace BankRestApi.Interfaces;

public interface ITokenService
{
    Task<Token> BuildToken(User user);
    
    Task<User?> TakeRefreshToken(string name, string refreshToken);
    
    string BuildRefreshToken();

    string BuildAccessToken(string name);
}