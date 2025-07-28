using BankRestApi.Models;
using BankRestApi.Models.DTOs.Requests;

namespace BankRestApi.Interfaces;

public interface IUserService
{
    User CreateUserAsync(AuthenticationRequest request);

    User GetByName(string name);
}