using BankRestApi.Application.DTOs.Accounts.Requests;
using BankRestApi.Application.DTOs.Accounts.Results;
using Account = BankRestApi.Domain.Entities.Account;

namespace BankRestApi.Application.Interfaces;

public interface IAccountRepository
{
    Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(GetAccountsQueryParameters queryParameters);
    
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Add(string name);
    
    Task<Account?> Update(Account account, decimal amount);
}