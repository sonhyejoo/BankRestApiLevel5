using System.Net;
using BankRestApi.Models.DTOs;

namespace BankRestApi.ExtensionMethods;

public static class AccountExtensionMethods
{
    public static AccountResult<Account> CreateResult(this Models.Account account) =>
        new(
            HttpStatusCode.OK,
            result: account.ToDto()
        );
    public static Account ToDto(this Models.Account account) => 
        new Account(account.Id, account.Name, account.Balance);
}