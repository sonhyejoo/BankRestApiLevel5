namespace BankRestApi.Models.DTOs;

public record TransferDetails(
    Account Sender,
    Account Recipient)
{
    public static readonly TransferDetails Empty = new TransferDetails(Account.Empty, Account.Empty);
}