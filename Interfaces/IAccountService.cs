using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;

namespace BankRestApi.Interfaces;

public interface IAccountService
{
    Task<BaseResult<PagedAccountsDtoResult>> GetAccounts(string? name,
        string sortBy,
        bool desc,
        int pageNumber,
        int pageSize);
    
    Task<BaseResult<Account>> Create(CreateAccount request);
    
    Task<BaseResult<Account>> Get(GetAccount request);
    
    Task<BaseResult<Account>> Deposit(Transaction request);
    
    Task<BaseResult<Account>> Withdraw(Transaction request);
    
    Task<BaseResult<TransferDetails>> Transfer(Transaction request);
    
    Task<BaseResult<ConvertedBalances>> ConvertBalances(ConvertCommand command);
}