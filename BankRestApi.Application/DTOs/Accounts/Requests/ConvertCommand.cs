namespace BankRestApi.Application.DTOs.Accounts.Requests;

public record ConvertCommand(Guid Id, List<string> Currencies);