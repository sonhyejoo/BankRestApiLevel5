namespace BankRestApi.Models.DTOs.Requests;

public record CreateUserRequest(string AccountName, string Password) : UserRequest(AccountName);