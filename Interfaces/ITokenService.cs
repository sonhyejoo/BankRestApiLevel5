using BankRestApi.Models.DTOs;
using User = BankRestApi.Models.User;

namespace BankRestApi.Interfaces;

public interface ITokenService
{
    Task<string?> TakeRefreshToken(string token, string name);

    string BuildRefreshToken();

    string BuildAccessToken(User user);
}