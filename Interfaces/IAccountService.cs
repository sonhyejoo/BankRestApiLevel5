using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public interface IAccountService
{
    Task<AccountResult<Account>> Create(CreateAccountRequest request);
    Task<AccountResult<Account>> Get(GetAccountRequest request);
    Task<AccountResult<decimal>> Deposit(TransactionRequest request);
    Task<AccountResult<decimal>> Withdraw(TransactionRequest request);
    Task<AccountResult<TransferBalances>> Transfer(TransactionRequest request);
}