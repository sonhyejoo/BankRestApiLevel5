using BankRestApi.Models;

namespace BankRestApi.Services;

public interface IAccountRepository
{
    Account GetById(Guid id);
    Account Insert(Account account);
    Account Update(Account account);
}