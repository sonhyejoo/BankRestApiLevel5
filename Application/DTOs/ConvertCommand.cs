namespace BankRestApi.Models.DTOs;

public record ConvertCommand(Guid Id, List<string> Currencies);