using BankRestApi.Application.DTOs.Accounts.Requests;
using BankRestApi.Application.DTOs.Accounts.Results;
using BankRestApi.Interfaces;
using BankRestApi.Models;
using BankRestApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Account = BankRestApi.Models.Account;

namespace BankRestApi.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(
        GetAccountsQueryParameters queryParameters)
    {
        var (name, sortBy, desc, pageNumber, pageSize) = queryParameters;
        var queryBuilder = _context.Accounts as IQueryable<Account>;
        
        if (!string.IsNullOrWhiteSpace(queryParameters.Name))
        {
            name = name.Trim();
            queryBuilder = queryBuilder.Where(a => a.Name == name);
        }

        sortBy = sortBy.Trim().ToLower();
        queryBuilder = sortBy switch
        {
            "name" => queryBuilder.OrderBy(a => a.Name),
            "balance" => queryBuilder.OrderBy(a => a.Balance),
            _ => queryBuilder
        };

        var totalItemCount = await queryBuilder.CountAsync();
        var pageData = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

        queryBuilder = queryBuilder
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
        var result = desc 
            ? await queryBuilder.Reverse().ToListAsync() 
            : await queryBuilder.ToListAsync();

        return (result, pageData);
    }

    public Task<Account?> GetById(Guid? id)
        => _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Account?> Add(string name)
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