using Account = BankRestApi.Domain.Entities.Account;

namespace Application.Interfaces;

public interface IAccountRepository
{
    Task<(IEnumerable<Account>, PaginationMetadata)> GetAccounts(GetAccountsQueryParameters queryParameters);
    
    Task<Account?> GetById(Guid? id);
    
    Task<Account?> Add(string name);
    
    Task<Account?> Update(Account account, decimal amount);
}