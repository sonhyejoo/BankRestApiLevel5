using Application.Interfaces;

namespace BankRestApi.Application.DTOs.Authentication;

public record CreateUserRequest(string Name, string Password) : IUserRequest;