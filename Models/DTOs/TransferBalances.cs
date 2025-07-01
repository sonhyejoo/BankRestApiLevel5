namespace BankRestApi.Models.DTOs;

public record TransferBalances(
    decimal Sender,
    decimal Receiver
)
{
    public static readonly TransferBalances Empty = new TransferBalances(0, 0);
}