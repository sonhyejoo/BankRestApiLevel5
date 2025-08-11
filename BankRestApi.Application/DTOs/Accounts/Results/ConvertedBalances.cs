namespace BankRestApi.Application.DTOs.Accounts.Results;

public record ConvertedBalances(
    Guid Id,
    string Name,
    decimal Balance,
    Dictionary<string, decimal> Balances)
    : Account(Id, Name, Balance);