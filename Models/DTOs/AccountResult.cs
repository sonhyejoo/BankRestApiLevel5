namespace BankRestApi.Models.DTOs;

public class AccountResult<T>(T result, string message = "")
{
    public T Result = result;
    public string Message = message;
    public bool Success = string.IsNullOrEmpty(message);
}