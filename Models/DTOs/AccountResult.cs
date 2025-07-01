namespace BankRestApi.Models.DTOs;

public class AccountResult<T>(T result, string errorMessage = "")
{
    public T Result => result;
    public string ErrorMessage => errorMessage;
    public bool IsSuccess => string.IsNullOrEmpty(errorMessage);

    public static AccountResult<T> GreaterThanZeroError(T result) =>
        new(result, "Please enter valid decimal amount greater than zero.");
    public static AccountResult<T> AccountNotFoundError(T result) =>
        new(result, "No account found with that id.");
}