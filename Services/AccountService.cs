using BankRestApi.Models;
using BankRestApi.Models.DTOs;
using Account = BankRestApi.Models.DTOs.Account;

namespace BankRestApi.Services;

public class AccountService : IAccountService
{
    private readonly AccountContext _context;

    public AccountService(AccountContext context)
    {
        _context = context;
    }

    public async Task<AccountResult<Account>> Create(CreateAccountRequest request)
    {
        var name = request.Name;
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            return new AccountResult<Account>
            (
                result: new Account{
                    Id = Guid.Empty,
                    Name = "",
                    Balance = 0
                },
                errorMessage: "Name cannot be empty or whitespace."
            );
        }

        var accountToAdd = new Models.Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            Balance = 0
        };
        
        _context.Accounts.Add(accountToAdd);
        await _context.SaveChangesAsync();

        return new AccountResult<Account>
        (
            result: new Account
            {
                Id = accountToAdd.Id,
                Name = accountToAdd.Name,
                Balance = accountToAdd.Balance
            }
        );
    }
}