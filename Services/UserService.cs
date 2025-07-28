using BankRestApi.Interfaces;
using BankRestApi.Models;
using BankRestApi.Models.DTOs.Requests;

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
    
    public User CreateUserAsync(AuthenticationRequest request)
    {
        var user = new User();
        user.AccountName = request.AccountName;
        user.HashedPassword = _passwordHelper.GeneratePassword(user, request.Password);

        _repository.Insert(user);

        return user;
    }

    public User GetByName(string name)
    {
        throw new NotImplementedException();
    }
}