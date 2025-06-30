namespace BankRestApi.Models.DTOs;

public record AccountResult<T>(
    T Result,
    string Message,
    bool Success
    );