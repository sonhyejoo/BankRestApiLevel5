using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public interface IAccountService
{
    Task<AccountResult<Account>> Create(CreateAccountRequest request);
}