﻿using System.Net;
using BankRestApi.ExtensionMethods;
using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
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
        var existingUser = await _repository.Get(request.Name);
        if (existingUser is not null || string.IsNullOrEmpty(request.Password))
        {
            return new BaseResult<User>(HttpStatusCode.BadRequest, "Name or password is invalid.");
        }
        var user = new Models.User
        {
            Name = request.Name
        };
        user.HashedPassword = _passwordHelper.GeneratePassword(user, request.Password);

        await _repository.Add(user);

        return new BaseResult<User>(HttpStatusCode.OK, user.ToDto(request.Password));
    }
}