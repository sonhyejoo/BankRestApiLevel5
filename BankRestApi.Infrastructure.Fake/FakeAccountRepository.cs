using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Domain.Entities;

namespace BankRestApi.Infrastructure.Fake;

public class FakeAccountRepository : IAccountRepository
{
    private static readonly Account? NullAccount = null;
    private readonly Dictionary<Guid, Account> _accounts;

    public FakeAccountRepository()
    {
        _accounts = new Dictionary<Guid, Account>();
    }
    
    public Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(GetAccountsQueryParameters queryParameters)
    {
        var (name, sortBy, desc, pageNumber, pageSize) = queryParameters;
        var queryBuilder = _accounts.Values.AsQueryable();
        
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

        var totalItemCount = queryBuilder.Count();
        var pageData = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

        queryBuilder = queryBuilder
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
        var result = desc 
            ? queryBuilder.Reverse().ToList() 
            : queryBuilder.ToList();

        return Task.FromResult<(IEnumerable<Account>, PaginationMetadata)>((result, pageData));
    }

    public Task<Account?> GetById(Guid? id)
    {
        if (!_accounts.ContainsKey((Guid)id))
        {
            return Task.FromResult(NullAccount);
        }
        
        return Task.FromResult(_accounts.FirstOrDefault(kvp => kvp.Key == (Guid)id).Value);
    }

    public Task<Account?> Add(string name)
    {
        var accountToAdd = new Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            Balance = 0
        };
        _accounts.Add(accountToAdd.Id, accountToAdd);

        return Task.FromResult(accountToAdd);
    }

    public Task<Account?> Update(Account account, decimal amount)
    {
        account.Balance += amount;
        _accounts[account.Id] = account;

        return Task.FromResult(account);
    }
}