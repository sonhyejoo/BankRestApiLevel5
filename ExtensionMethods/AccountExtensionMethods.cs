using System.Net;
using BankRestApi.Models.DTOs;

namespace BankRestApi.ExtensionMethods;

public static class AccountExtensionMethods
{
    public static AccountResult<Account> CreateResult(this Models.Account account) =>
        new(
            HttpStatusCode.OK,
            result: new Account
            {
                Id = account.Id,
                Name = account.Name,
                Balance = account.Balance
            }
        );
    public static Account ToDto(this Models.Account account) => 
        new Account
        {
            Id = account.Id, 
            Name = account.Name, 
            Balance = account.Balance
        };
}