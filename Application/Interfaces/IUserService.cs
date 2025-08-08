using Application.DTOs;
using BankRestApi.Application.DTOs.Authentication;
using User = BankRestApi.Application.DTOs.Authentication.User;

namespace Application.Interfaces;

public interface IUserService
{
    Task<BaseResult<User>> CreateUserAsync(CreateUserRequest request);
}