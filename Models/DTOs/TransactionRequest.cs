namespace BankRestApi.Models.DTOs;

public record TransactionRequest(
    decimal Amount,
    Guid Id,
    Guid? RecipientId
    );