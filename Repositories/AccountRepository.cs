using BankRestApi.Interfaces;
using BankRestApi.Models;
using BankRestApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Account = BankRestApi.Models.Account;

namespace BankRestApi.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountContext _context;

    public AccountRepository(AccountContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(
        string? name,
        string sort,
        bool desc,
        int pageNumber,
        int pageSize)
    {
        var queryBuilder = _context.Accounts as IQueryable<Account>;
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            queryBuilder = queryBuilder.Where(a => a.Name == name);
        }

        sort = sort.Trim().ToLower();
        switch (sort)
        {
            case "name":
                queryBuilder = queryBuilder.OrderBy(a => a.Name);
                break;
            case "balance":
                queryBuilder = queryBuilder.OrderBy(a => a.Balance);
                break;
        }

        var totalItemCount = await queryBuilder.CountAsync();
        var paginationMetaData = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

        queryBuilder = queryBuilder
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
        var result = desc 
            ? await queryBuilder.Reverse().ToListAsync() 
            : await queryBuilder.ToListAsync();

        return (result, paginationMetaData);
    }

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