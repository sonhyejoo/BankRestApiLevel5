using BankRestApi.Application.DTOs.Interfaces;

namespace BankRestApi.Application.DTOs.Requests;

public record LoginRequest(string Name, string Password) : IUserRequest;