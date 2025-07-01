using System.ComponentModel.DataAnnotations;

namespace BankRestApi.Models;

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public decimal Balance { get; set; }
}