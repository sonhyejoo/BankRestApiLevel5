using System.Net;
using BankRestApi.Application.DTOs;
using Account = BankRestApi.Application.DTOs.Accounts.Account;
using EntityAccount = BankRestApi.Domain.Entities.Account;
using User = BankRestApi.Application.DTOs.Authentication.User;
using EntityUser = BankRestApi.Domain.Entities.User;

namespace BankRestApi.Application;

public static class ExtensionMethods
{
    public static BaseResult<Account> CreateResult(this EntityAccount account)
        => new(HttpStatusCode.OK, result: account.ToDto());
    
    public static Account ToDto(this EntityAccount account)
        => new (account.Id, account.Name, account.Balance);

    public static User ToDto(this EntityUser user, string password)
        => new(user.Name, password);
}