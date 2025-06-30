namespace BankRestApi.Models;

public class Account(Guid id, string name, decimal balance)
{
    public int InternalId { get; set; }
    public Guid Id { get; } = id;
    public string Name { get; set; } = name;
    public decimal Balance { get; set; } = balance;
}