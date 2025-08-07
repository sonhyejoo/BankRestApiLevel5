using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
using Account = BankRestApi.Models.Account;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(GetAccountsQueryParameters queryParameters);
    
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Add(string name);
    
    Task<Account?> Update(Account account, decimal amount);
}