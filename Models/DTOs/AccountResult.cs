using System.Net;

namespace BankRestApi.Models.DTOs;

public class AccountResult<T>
{
    public HttpStatusCode StatusCode { get; }
    public T? Result { get; }
    public string ErrorMessage { get; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public AccountResult(HttpStatusCode statusCode, T result, string errorMessage = "")
    {
        Result = result;
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
    public AccountResult(HttpStatusCode statusCode, string errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }

    public static AccountResult<T> NonpositiveAmountError() =>
        new(HttpStatusCode.BadRequest, "Please enter valid decimal amount greater than zero.");
    public static AccountResult<T> NotFoundError() =>
        new(HttpStatusCode.NotFound, "No account found with that ID.");
    public static AccountResult<T> InsufficientFundsError() =>
        new(HttpStatusCode.BadRequest, "Insufficient funds.");
    public static AccountResult<T> EmptyNameError() =>
        new(HttpStatusCode.BadRequest, "Name cannot be empty or whitespace.");
    public static AccountResult<T> DuplicateIdError() =>
        new(HttpStatusCode.BadRequest, "Duplicate ids given for sender and recipient.");
}