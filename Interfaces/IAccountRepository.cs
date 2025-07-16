using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Insert(Account account);
    
    Task<Account?> Update(Account account, decimal amount);
}