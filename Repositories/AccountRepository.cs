using BankRestApi.Models;
using BankRestApi.Services;

namespace BankRestApi.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountContext _context;

    public AccountRepository(AccountContext context)
    {
        _context = context;
    }
    
    public async Task<Account?> GetById(Guid id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> Insert(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }

    public async Task<Account?> Update(Account account)
    {
        var accountInDb = await _context.Accounts.FindAsync(account.Id);
        if (accountInDb is not null)
        {
            accountInDb.Name = account.Name;
            accountInDb.Balance = account.Balance;
        }
        await _context.SaveChangesAsync();

        return accountInDb;
    }
}