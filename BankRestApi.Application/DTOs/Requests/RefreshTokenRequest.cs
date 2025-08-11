using BankRestApi.Application.DTOs.Interfaces;

namespace BankRestApi.Application.DTOs.Requests;

public record RefreshTokenRequest(string Name, string RefreshToken) : IUserRequest;