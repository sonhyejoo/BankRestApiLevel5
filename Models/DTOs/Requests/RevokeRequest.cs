namespace BankRestApi.Models.DTOs.Requests;

public record RevokeRequest(string AccountName, string RefreshToken) : UserRequest(AccountName);