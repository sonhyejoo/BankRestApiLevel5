namespace BankRestApi.Models.DTOs;

public record Account(
    Guid Id,
    string Name,
    decimal Balance
);