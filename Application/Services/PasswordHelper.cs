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
        => _passwordHasher.HashPassword(user, password);

    public bool PasswordMatches(User user, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, providedPassword);

        return result == PasswordVerificationResult.Success;
    }
}