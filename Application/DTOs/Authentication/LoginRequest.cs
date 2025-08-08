using Application.Interfaces;

namespace BankRestApi.Application.DTOs.Authentication;

public record LoginRequest(string Name, string Password) : IUserRequest;