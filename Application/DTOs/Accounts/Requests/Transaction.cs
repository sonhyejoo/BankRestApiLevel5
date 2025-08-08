namespace Application.DTOs.Accounts.Requests;

public record Transaction(decimal Amount, Guid Id, Guid? RecipientId = null);