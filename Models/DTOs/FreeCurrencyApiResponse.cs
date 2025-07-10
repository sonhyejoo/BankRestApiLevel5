namespace BankRestApi.Models.DTOs;

public record FreeCurrencyApiResponse(
    Dictionary<string, decimal> data
    );