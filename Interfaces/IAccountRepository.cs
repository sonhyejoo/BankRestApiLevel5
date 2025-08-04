using BankRestApi.Models.DTOs;
using Account = BankRestApi.Models.Account;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(string? name,
        string sortBy,
        bool desc,
        int pageNumber,
        int pageSize);
    
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Add(string name);
    
    Task<Account?> Update(Account account, decimal amount);
}