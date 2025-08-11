using BankRestApi.Application.DTOs.Authentication;
using User = BankRestApi.Domain.Entities.User;

namespace BankRestApi.Application.Interfaces;

public interface ITokenService
{
    Task<Token> BuildToken(User user);
    
    Task<User?> TakeRefreshToken(string name, string refreshToken);
}