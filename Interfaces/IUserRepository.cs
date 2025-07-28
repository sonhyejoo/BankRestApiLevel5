using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IUserRepository
{
    Task Insert(User user);

    Task<User?> GetByName(string name);
}