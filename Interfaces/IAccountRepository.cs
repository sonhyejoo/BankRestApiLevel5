using BankRestApi.Models;

namespace BankRestApi.Services;

public interface IAccountRepository
{
    Task<Account?> TryGetById(Guid? id);
    Task<Account> TryInsert(Account account);
    Task<Account?> TryUpdate(Account account);
}