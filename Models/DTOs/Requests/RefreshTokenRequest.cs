namespace BankRestApi.Models.DTOs.Requests;

public record RefreshTokenRequest(string AccountName, string RefreshToken);