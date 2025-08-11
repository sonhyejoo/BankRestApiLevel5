using BankRestApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankRestApi.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;
    
    public DbSet<User> Users { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 2);
    }
}