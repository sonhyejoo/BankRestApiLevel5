using BankRestApi.Application.Interfaces;

namespace BankRestApi.Application.DTOs.Authentication;

public record RefreshTokenRequest(string Name, string RefreshToken) : IUserRequest;