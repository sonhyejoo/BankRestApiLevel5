using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IUserRepository
{
    Task<User?> Insert(User user);

    Task<User?> GetByName(string name);
}