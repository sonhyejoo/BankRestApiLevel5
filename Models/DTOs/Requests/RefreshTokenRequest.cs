namespace BankRestApi.Models.DTOs.Requests;

public record RefreshTokenRequest(string Name, string RefreshToken) : UserRequest(RefreshToken);