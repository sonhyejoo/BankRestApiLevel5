using BankRestApi.Models;
using BankRestApi.Models.DTOs.Requests;

namespace BankRestApi.Interfaces;

public interface IUserService
{
    User CreateUserAsync(AuthenticationRequest request);

    Task<User?> GetByName(string name);
}