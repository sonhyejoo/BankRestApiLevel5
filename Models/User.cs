namespace BankRestApi.Models;

public class User{
    public int Id { get; set; }
    public string AccountName { get; set; }
    public string HashedPassword { get; set; }
}