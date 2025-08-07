namespace BankRestApi.Models.DTOs.Requests;

public record CreateUserRequest(string Name, string Password) : UserRequest(Name);