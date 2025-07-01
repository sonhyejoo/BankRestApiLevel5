namespace BankRestApi.Models.DTOs;

public class AccountResult<T>()
{
    public T Result { get; set; }
    public string Message { get; set; } = "";
    public bool Success { get; set; } = string.IsNullOrEmpty(message);
}