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
    
    public async Task<Account?> TryGetById(Guid? id)
    {
        var result =  await _context.Accounts.FindAsync(id);
        if (result is null)
        {
            throw new KeyNotFoundException("No account found with that ID.");
        }

        return result;
    }

    public async Task<Account> TryInsert(Account account)
    {
        var result = _context.Accounts.Add(account);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DbUpdateConcurrencyException("An internal service error occurred.", ex);
        }

        return result.Entity;
    }

    public async Task<Account?> TryUpdate(Account account)
    {
        var result = await TryGetById(account.Id);
        
        if (result is not null)
        {
            throw new KeyNotFoundException("No account found with that ID.");
        }

        if (account.Balance < 0)
        {
            throw new InvalidOperationException("Invalid transaction.");
        }
        result.Name = account.Name;
        result.Balance = account.Balance;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DbUpdateConcurrencyException("An internal service error occurred.", ex);
        }

        return result;
    }
}