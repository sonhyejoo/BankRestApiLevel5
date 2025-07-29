namespace BankRestApi.Models.DTOs;

public record User(
    string AccountName,
    string Password,
    string? RefreshToken = null,
    DateTime? RefreshTokenExpiry = null);