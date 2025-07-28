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
    private readonly IConfiguration _config;

    public UserService(IUserRepository userRepository, IPasswordHelper passwordHelper, IConfiguration config)
    {
        _repository = userRepository;
        _passwordHelper = passwordHelper;
        _config = config;
    }
    
    public async Task<BaseResult<User>> CreateUserAsync(CreateUserRequest request)
    {
        var existingUser = await _repository.GetByName(request.AccountName);
        if (existingUser is not null)
        {
            return new BaseResult<User>(HttpStatusCode.BadRequest, "Please choose different name.");
        }
        var user = new Models.User
        {
            AccountName = request.AccountName
        };
        user.HashedPassword = _passwordHelper.GeneratePassword(user, request.Password);

        await _repository.Insert(user);

        return new BaseResult<User>(HttpStatusCode.OK, user.ToDto(request.Password));
    }
    
    public async Task<BaseResult<User>> ValidateUserCredentials(AuthenticationRequest request)
    {
        var existingUser = await _repository.GetByName(request.AccountName);
        if (existingUser is null)
        {
            return new BaseResult<User>(HttpStatusCode.NotFound, "Account not found.");
        }

        if (!_passwordHelper.PasswordMatches(existingUser, request.Password))
        {
            return new BaseResult<User>(HttpStatusCode.BadRequest, "Invalid name or password.");
        }
        
        var securityKey = new SymmetricSecurityKey(
            Convert.FromBase64String(_config["Authentication:SecretForKey"]));
        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>()
        {
            new Claim("sub", existingUser.Id.ToString()),
            new Claim("name", existingUser.AccountName)
        };

        var jwtSecurityToken = new JwtSecurityToken(
            _config["Authentication:Issuer"],
            _config["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new BaseResult<User>(HttpStatusCode.OK, tokenToReturn);
    }
}