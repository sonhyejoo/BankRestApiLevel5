namespace BankRestApi.Models.DTOs;

public class AccountResult<T>(T result, string? message = "")
{
    public readonly T Result = result;
    public readonly string Message = message;
    public readonly bool Success = string.IsNullOrEmpty(message);
}