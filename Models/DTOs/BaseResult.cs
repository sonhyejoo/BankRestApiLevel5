using System.Net;

namespace BankRestApi.Models.DTOs;

public class BaseResult<T>
{
    public HttpStatusCode? StatusCode { get; }
    
    public T? Result { get; }
    
    public string ErrorMessage { get; }
    
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public BaseResult(HttpStatusCode statusCode, T result, string errorMessage = "")
    {
        Result = result;
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
    
    public BaseResult(HttpStatusCode? statusCode, string errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
    
    public static BaseResult<T> NonpositiveAmountError() =>
        new(HttpStatusCode.BadRequest, "Please enter valid decimal amount greater than zero.");
    
    public static BaseResult<T> NotFoundError() =>
        new(HttpStatusCode.NotFound, "No account found with that ID.");
    
    public static BaseResult<T> InsufficientFundsError() =>
        new(HttpStatusCode.BadRequest, "Insufficient funds.");
    
    public static BaseResult<T> EmptyNameError() =>
        new(HttpStatusCode.BadRequest, "Name cannot be empty or whitespace.");
    
    public static BaseResult<T> DuplicateIdError() =>
        new(HttpStatusCode.BadRequest, "Duplicate IDs given for sender and recipient.");
}