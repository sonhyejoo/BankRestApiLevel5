using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public interface IAccountService
{
    Task<AccountResult<Account>> Create(CreateAccountRequest request);
    Task<AccountResult<Account>> Get(GetAccountRequest request);
    Task<AccountResult<Account>> Deposit(TransactionRequest request);
    Task<AccountResult<Account>> Withdraw(TransactionRequest request);
    Task<AccountResult<TransferDetails>> Transfer(TransactionRequest request);
}