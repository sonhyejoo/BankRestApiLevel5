namespace BankRestApi.Models.DTOs;

public class AccountResult<T>
{
    public T? Result { get; }
    public string ErrorMessage { get; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public AccountResult(T result, string errorMessage = "")
    {
        Result = result;
        ErrorMessage = errorMessage;
    }
    public AccountResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public static AccountResult<T> GreaterThanZeroError(T result) =>
        new(result, "Please enter valid decimal amount greater than zero.");
    public static AccountResult<T> NotFoundError(T result) =>
        new(result, "No account found with that ID.");
    public static AccountResult<T> InsufficientFundsError(T result) =>
        new(result, "Insufficient funds.");
    public static AccountResult<T> EmptyNameError(T result) =>
        new(result, "Name cannot be empty or whitespace.");
    public static AccountResult<T> DuplicateIdError(T result) =>
        new(result, "Duplicate ids given for sender and recipient.");
}