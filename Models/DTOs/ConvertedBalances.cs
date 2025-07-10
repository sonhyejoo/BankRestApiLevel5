namespace BankRestApi.Models.DTOs;

public record ConvertedBalances(
    Guid Id,
    string Name,
    decimal Balance,
    Dictionary<string, decimal> Balances
    ) : Account(Id, Name, Balance);