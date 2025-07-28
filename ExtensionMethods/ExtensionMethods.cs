using System.Net;
using BankRestApi.Models.DTOs;
using Account = BankRestApi.Models.DTOs.Account;

namespace BankRestApi.ExtensionMethods;

public static class ExtensionMethods
{
    public static BaseResult<Account> CreateResult(this Models.Account account) =>
        new(HttpStatusCode.OK, result: account.ToDto());
    public static Account ToDto(this Models.Account account)
        => new (account.Id, account.Name, account.Balance);

    public static User ToDto(this Models.User user, string password)
        => new(user.AccountName, password);
}