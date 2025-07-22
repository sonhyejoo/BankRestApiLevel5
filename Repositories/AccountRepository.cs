using BankRestApi.Interfaces;
using BankRestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankRestApi.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountContext _context;

    public AccountRepository(AccountContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Account>> GetAccounts()
        => await _context.Accounts.ToListAsync();

    public Task<Account?> GetById(Guid? id)
        => _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Account?> Insert(string name)
    {
        var accountToAdd = new Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            Balance = 0
        };
        var result = await _context.Accounts.AddAsync(accountToAdd);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Account?> Update(Account account, decimal amount)
    {
        account.Balance += amount;
        await _context.SaveChangesAsync();

        return account;
    }
}