namespace BankRestApi.Models.DTOs;

public record TransferDetails(
    Account Sender,
    Account Receiver)
{
    public static readonly TransferDetails Empty = new TransferDetails(Account.Empty, Account.Empty);
}