namespace BankRestApi.Application.DTOs.Accounts.Results;

public record CurrencyApiResponse(Dictionary<string, decimal> Data);