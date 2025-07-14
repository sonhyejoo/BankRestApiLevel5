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
    
    public async Task<Account?> GetById(Guid? id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> Insert(Account account)
    {
        var result = _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Account?> Update(Account account)
    {
        var result = await _context.Accounts.FindAsync(account.Id);
        if (result is not null)
        {
            result.Name = account.Name;
            result.Balance = account.Balance;
        }
        await _context.SaveChangesAsync();

        return result;
    }
}