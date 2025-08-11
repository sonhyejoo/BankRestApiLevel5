namespace BankRestApi.Application.DTOs.Results;

public record CurrencyApiResponse(Dictionary<string, decimal> Data);