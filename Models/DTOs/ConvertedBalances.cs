namespace BankRestApi.Models.DTOs;

public class ConvertedBalances() : Account
{
    public Dictionary<string, decimal> Balances { get; set; } = new Dictionary<string, decimal>();
}