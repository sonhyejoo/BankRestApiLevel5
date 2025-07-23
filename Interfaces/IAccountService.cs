using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;

namespace BankRestApi.Interfaces;

public interface IAccountService
{
    Task<AccountResult<AccountsAndPageData>> GetAccounts(string? name,
        string sort,
        bool desc,
        int pageNumber,
        int pageSize);
    
    Task<AccountResult<Account>> Create(CreateAccount request);
    
    Task<AccountResult<Account>> Get(GetAccount request);
    
    Task<AccountResult<Account>> Deposit(Transaction request);
    
    Task<AccountResult<Account>> Withdraw(Transaction request);
    
    Task<AccountResult<TransferDetails>> Transfer(Transaction request);
    
    Task<AccountResult<ConvertedBalances>> ConvertBalances(ConvertCommand command);
}