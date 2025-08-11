namespace BankRestApi.Application.DTOs.Results;

public record TransferDetails(Account Sender, Account Recipient);