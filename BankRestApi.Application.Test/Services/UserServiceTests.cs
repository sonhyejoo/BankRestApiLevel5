using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Infrastructure.Fake;
using User = BankRestApi.Application.DTOs.User;

namespace BankRestApi.Application.Test.Services;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHelper _passwordHelper;

    public UserServiceTests()
    {
        _userRepository = new FakeUserRepository();
        _passwordHelper = new FakePasswordHelper();
    }

    [Fact]
    public async Task CreateUserAsync_ValidUser_CreatesUser()
    {
        var userService = CreateDefaultUserService();
        var request = new CreateUserRequest("name", "password");

        var result = await userService.CreateUserAsync(request);

        Assert.Equivalent(result.Result, new User(request.Name, request.Password));
    }
    
    [Fact]
    public async Task CreateUserAsync_EmptyName_ReturnsBadRequest()
    {
        var userService = CreateDefaultUserService();
        var request = new CreateUserRequest("", "password");

        var result = await userService.CreateUserAsync(request);

        Assert.Equivalent(
            result,
            new BaseResult<User>(HttpStatusCode.BadRequest, "Name or password is invalid."));
    }

    [Fact]
    public async Task CreateUserAsync_EmptyPassword_ReturnsBadRequest()
    {
        var userService = CreateDefaultUserService();
        var request = new CreateUserRequest("name", "");

        var result = await userService.CreateUserAsync(request);

        Assert.Equivalent(
            result,
            new BaseResult<User>(HttpStatusCode.BadRequest, "Name or password is invalid."));
    }

    private UserService CreateDefaultUserService() => new(_userRepository, _passwordHelper);
}