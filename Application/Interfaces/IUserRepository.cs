namespace Application.Interfaces;

public interface IUserRepository
{
    Task<User?> Add(User user);

    Task<User?> Get(string name);
    
    Task Update(User user, string? refreshToken, DateTime? refreshTokenExpiry);
}