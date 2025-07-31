namespace BankRestApi.Models.DTOs.Requests;

public record RevokeRequest(string Name, string RefreshToken) : UserRequest(Name);