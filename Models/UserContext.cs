using Microsoft.EntityFrameworkCore;

namespace BankRestApi.Models;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }
}