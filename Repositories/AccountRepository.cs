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

    public async Task<(bool, Account?)> TryGetById(Guid? id)
    {
        var result = await _context.Accounts.FindAsync(id);
        return result is null ? (false, null) : (true, result);
    }

    public async Task<(bool, Account?)> TryInsert(Account account)
    {
        var result = _context.Accounts.Add(account);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return (false, null);
        }

        return (true, result.Entity);
    }

    public async Task<(bool, Account?)> TryUpdate(Account account)
    {
        var result = await _context.Accounts.FindAsync(account.Id);

        if (result is null || account.Balance < 0)
        {
            return (false, null);
        }
        result.Name = account.Name;
        result.Balance = account.Balance;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return (false, null);
        }

        return (true, result);
    }
}