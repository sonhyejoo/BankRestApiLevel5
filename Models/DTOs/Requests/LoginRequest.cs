namespace BankRestApi.Models.DTOs.Requests;

public record LoginRequest(string Name, string Password) : UserRequest(Name);