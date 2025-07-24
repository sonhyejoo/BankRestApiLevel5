namespace BankRestApi.Models.DTOs.Requests;

public record AuthenticationRequest(string AccountName, string Password);