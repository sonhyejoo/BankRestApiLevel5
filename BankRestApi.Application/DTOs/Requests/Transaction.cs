namespace BankRestApi.Application.DTOs.Requests;

public record Transaction(decimal Amount, Guid Id, Guid? RecipientId = null);