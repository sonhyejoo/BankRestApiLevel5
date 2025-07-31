using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BankRestApi.ExtensionMethods;
using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
using Microsoft.IdentityModel.Tokens;
using User = BankRestApi.Models.DTOs.User;

namespace BankRestApi.Services;

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
        var existingUser = await _repository.GetByName(request.Name);
        if (existingUser is not null)
        {
            return new BaseResult<User>(HttpStatusCode.BadRequest, "Please choose different name.");
        }
        var user = new Models.User
        {
            Name = request.Name
        };
        user.HashedPassword = _passwordHelper.GeneratePassword(user, request.Password);

        await _repository.Insert(user);

        return new BaseResult<User>(HttpStatusCode.OK, user.ToDto(request.Password));
    }
}