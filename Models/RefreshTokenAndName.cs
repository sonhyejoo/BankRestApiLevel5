namespace BankRestApi.Models;

public class RefreshTokenAndName
{
    public int Id { get; set; }

    public string RefreshToken { get; set; }

    public string AccountName { get; set; }
}