using BankRestApi.Application.DTOs.Authentication;
using User = BankRestApi.Models.User;

namespace Application.Interfaces;

public interface ITokenService
{
    Task<Token> BuildToken(User user);
    
    Task<User?> TakeRefreshToken(string name, string refreshToken);
}