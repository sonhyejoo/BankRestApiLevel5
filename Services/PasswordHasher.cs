using BankRestApi.Interfaces;
using BankRestApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BankRestApi.Services;

public class PasswordHelper : IPasswordHelper
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordHelper(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }
    
    public string GeneratePassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool PasswordMatches(User user, string providedPassword, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, providedPassword, passwordHash);

        return result == PasswordVerificationResult.Success;
    }
}