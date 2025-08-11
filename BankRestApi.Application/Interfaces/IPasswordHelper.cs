using BankRestApi.Domain.Entities;

namespace BankRestApi.Application.Interfaces;

public interface IPasswordHelper
{
    string GeneratePassword(User user, string password);
    
    bool PasswordMatches(User user, string providedPassword);
}