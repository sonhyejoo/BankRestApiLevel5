namespace BankRestApi.Models.DTOs;

public record Transaction(decimal Amount, Guid Id, Guid? RecipientId = null);