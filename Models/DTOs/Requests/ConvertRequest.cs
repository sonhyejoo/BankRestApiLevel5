namespace BankRestApi.Models.DTOs.Requests;

public record ConvertRequest(
    Guid Id,
    string? Currencies
    ) : GetAccount(Id);