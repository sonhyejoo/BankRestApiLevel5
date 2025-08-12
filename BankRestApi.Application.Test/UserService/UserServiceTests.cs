using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.Interfaces;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;
using User = BankRestApi.Application.DTOs.User;

namespace BankRestApi.Application.Test.UserService;

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
        var userService = new Services.UserService(_userRepository, _passwordHelper);
        var request = new CreateUserRequest("name", "password");

        var result = await userService.CreateUserAsync(request);

        Assert.Equivalent(result.Result, new User(request.Name, request.Password));
    }
    
    [Fact]
    public async Task CreateUserAsync_EmptyName_ReturnsBadRequest()
    {
        var userService = new Services.UserService(_userRepository, _passwordHelper);
        var request = new CreateUserRequest("", "password");

        var result = await userService.CreateUserAsync(request);

        Assert.Equal(result.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserAsync_EmptyPassword_ReturnsBadRequest()
    {
        var userService = new Services.UserService(_userRepository, _passwordHelper);
        var request = new CreateUserRequest("name", "");

        var result = await userService.CreateUserAsync(request);

        Assert.Equal(result.StatusCode, HttpStatusCode.BadRequest);
    }
}