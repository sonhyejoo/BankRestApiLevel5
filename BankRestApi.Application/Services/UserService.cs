using System.Net;
using BankRestApi.Application.DTOs;
using BankRestApi.Application.DTOs.Authentication;
using BankRestApi.Application.Interfaces;
using User = BankRestApi.Application.DTOs.Authentication.User;

namespace BankRestApi.Application.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHelper _passwordHelper;

    public UserService(IUserRepository userRepository, IPasswordHelper passwordHelper)
    {
        _repository = userRepository;
        _passwordHelper = passwordHelper;
    }
    
    public async Task<BaseResult<User>> CreateUserAsync(CreateUserRequest request)
    {
        var existingUser = await _repository.Get(request.Name);
        if (existingUser is not null || string.IsNullOrEmpty(request.Password))
        {
            return new BaseResult<User>(HttpStatusCode.BadRequest, "Name or password is invalid.");
        }
        var user = new BankRestApi.Domain.Entities.User
        {
            Name = request.Name
        };
        user.HashedPassword = _passwordHelper.GeneratePassword(user, request.Password);

        await _repository.Add(user);

        return new BaseResult<User>(HttpStatusCode.OK, user.ToDto(request.Password));
    }
}