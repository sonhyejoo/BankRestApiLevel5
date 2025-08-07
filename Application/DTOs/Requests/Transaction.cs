namespace BankRestApi.Models.DTOs.Requests;

public record Transaction(decimal Amount, Guid Id, Guid? RecipientId = null);