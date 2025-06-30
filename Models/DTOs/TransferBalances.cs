namespace BankRestApi.Models.DTOs;

public record TransferBalances(
    decimal Sender,
    decimal Receiver
    );