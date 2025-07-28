using Microsoft.EntityFrameworkCore;

namespace BankRestApi.Models;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;
    
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<RefreshTokenAndName> RefreshTokensAndNames { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}