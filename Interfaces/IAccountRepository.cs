using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAccounts(
        string? name,
        string sort,
        bool desc,
        int pageNumber,
        int pageSize);
    
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Insert(string name);
    
    Task<Account?> Update(Account account, decimal amount);
}