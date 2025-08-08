namespace BankRestApi.Application.DTOs.Authentication;

public record Token(string AccessToken, string RefreshToken);