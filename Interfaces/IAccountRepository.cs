using BankRestApi.Models.DTOs;
using Account = BankRestApi.Models.Account;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(string? name,
        string sort,
        bool desc,
        int pageNumber,
        int pageSize);
    
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Insert(string name);
    
    Task<Account?> Update(Account account, decimal amount);
}