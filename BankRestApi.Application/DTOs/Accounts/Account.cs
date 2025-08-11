namespace BankRestApi.Application.DTOs.Accounts;

public record Account(Guid Id, string Name, decimal Balance);