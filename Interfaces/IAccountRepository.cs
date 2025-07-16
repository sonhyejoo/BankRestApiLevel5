using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Insert(string name);
    
    Task<Account?> Update(Account account, decimal amount);
}