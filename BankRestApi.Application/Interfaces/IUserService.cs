using BankRestApi.Application.DTOs;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using User = BankRestApi.Application.DTOs.User;

namespace BankRestApi.Application.Interfaces;

public interface IUserService
{
    Task<BaseResult<User>> CreateUserAsync(CreateUserRequest request);
}