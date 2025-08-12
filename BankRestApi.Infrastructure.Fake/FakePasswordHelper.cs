using BankRestApi.Application.Interfaces;
using BankRestApi.Domain.Entities;

namespace BankRestApi.Infrastructure.Fake;

public class FakePasswordHelper : IPasswordHelper
{
    public string GeneratePassword(User user, string password) => password;

    public bool PasswordMatches(User user, string providedPassword) => user.HashedPassword == providedPassword;
}