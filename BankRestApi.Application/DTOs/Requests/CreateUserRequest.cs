using BankRestApi.Application.Interfaces;

namespace BankRestApi.Application.DTOs.Requests;

public record CreateUserRequest(string Name, string Password) : IUserRequest;