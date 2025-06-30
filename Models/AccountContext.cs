using Microsoft.EntityFrameworkCore;

namespace BankRestApi.Models;

public class AccountContext : DbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;

    public AccountContext(DbContextOptions<AccountContext> options)
        : base(options)
    {
    }
}