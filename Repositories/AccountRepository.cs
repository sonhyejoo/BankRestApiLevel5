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

    public Task<Account?> GetById(Guid? id)
        => _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Account?> Insert(Account account)
    {
        var result = await _context.Accounts.AddAsync(account);
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