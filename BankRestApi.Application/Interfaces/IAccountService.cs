using BankRestApi.Application.DTOs;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;

namespace BankRestApi.Application.Interfaces;

public interface IAccountService
{
    Task<BaseResult<PagedAccountsDtoResult>> GetAccounts(GetAccountsQueryParameters queryParameters);
    
    Task<BaseResult<Account>> Create(CreateAccount request);
    
    Task<BaseResult<Account>> Get(GetAccount request);
    
    Task<BaseResult<Account>> Deposit(Transaction request);
    
    Task<BaseResult<Account>> Withdraw(Transaction request);
    
    Task<BaseResult<TransferDetails>> Transfer(Transaction request);
    
    Task<BaseResult<ConvertedBalances>> ConvertBalances(ConvertCommand command);
}