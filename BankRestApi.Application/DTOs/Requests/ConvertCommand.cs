namespace BankRestApi.Application.DTOs.Requests;

public record ConvertCommand(Guid Id, List<string> Currencies);