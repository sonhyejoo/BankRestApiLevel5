using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
using User = BankRestApi.Models.DTOs.User;

namespace BankRestApi.Interfaces;

public interface IUserService
{
    Task<BaseResult<User>> CreateUserAsync(CreateUserRequest request);
}