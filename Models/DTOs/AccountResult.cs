namespace BankRestApi.Models.DTOs;

public class AccountResult<T>(T result, string errorMessage = "")
{
    public T Result => result;
    public string ErrorMessage => errorMessage;
    public bool IsSuccess => string.IsNullOrEmpty(errorMessage);

    public static AccountResult<T> GreaterThanZeroError(T result) =>
        new(result, "Please enter valid decimal deposit amount greater than zero.");
}