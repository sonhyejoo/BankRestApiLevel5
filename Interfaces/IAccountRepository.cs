using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<Account?> TryGetById(Guid? id);
    Task<Account> TryInsert(Account account);
    Task<Account?> TryUpdate(Account account);
}