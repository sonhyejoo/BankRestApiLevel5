using BankRestApi.Models;

namespace BankRestApi.Interfaces;

public interface IAccountRepository
{
    Task<(bool, Account?)> TryGetById(Guid? id);
    Task<(bool, Account?)> TryInsert(Account account);
    Task<(bool, Account?)> TryUpdate(Account account);
}