using BankRestApi.Application.Interfaces;

namespace BankRestApi.Application.DTOs.Requests;

public record RevokeRequest(string Name, string RefreshToken) : IUserRequest;