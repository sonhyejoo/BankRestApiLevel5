namespace BankRestApi.Models;

public record User(
    int Id,
    string AccountName,
    string HashedPassword);