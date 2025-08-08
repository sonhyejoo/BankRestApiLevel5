using Application.Interfaces;

namespace BankRestApi.Application.DTOs.Authentication;

public record RevokeRequest(string Name, string RefreshToken) : IUserRequest;