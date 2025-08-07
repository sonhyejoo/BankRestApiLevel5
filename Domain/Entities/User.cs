namespace BankRestApi.Domain.Entities;

public class User{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string HashedPassword { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiry { get; set; }
}