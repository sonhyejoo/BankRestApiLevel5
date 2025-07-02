namespace BankRestApi.Models.DTOs;

public class Account
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = "";
    public decimal Balance { get; set; } = 0;
}