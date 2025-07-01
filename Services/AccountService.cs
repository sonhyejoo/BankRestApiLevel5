using BankRestApi.Models;
using BankRestApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
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

    public async Task<AccountResult<Account>> Get(GetAccountRequest request)
    {
        var foundAccount = await  _context.Accounts.FindAsync(request.Id);

        if (foundAccount == null)
        {
            return new AccountResult<Account>
            (
                result: new Account{
                    Id = Guid.Empty,
                    Name = "",
                    Balance = 0
                },
                errorMessage: "No account found with that id."
            );
        }
        
        return new AccountResult<Account>
        (
            result: new Account
            {
                Id = foundAccount.Id,
                Name = foundAccount.Name,
                Balance = foundAccount.Balance
            }
        );
    }
    
    public async Task<AccountResult<decimal>> Deposit(TransactionRequest request)
    {
        var foundAccount = await  _context.Accounts.FindAsync(request.Id);

        if (foundAccount == null)
        {
            return new AccountResult<decimal>
            (
                result: 0,
                errorMessage: "No account found with that id."
            );
        }

        if (request.Amount <= 0)
        {
            return new AccountResult<decimal>(
                result: 0, 
                errorMessage: "Please enter valid decimal deposit amount greater than zero."
            );
        }
        
        foundAccount.Balance += request.Amount;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<decimal>(
                result: 0, 
                errorMessage: ex.Message
            );
        }
        
        return new AccountResult<decimal>(result: foundAccount.Balance);
    }
    
    public async Task<AccountResult<decimal>> Withdraw(TransactionRequest request)
    {
        var foundAccount = await  _context.Accounts.FindAsync(request.Id);

        if (foundAccount == null)
        {
            return new AccountResult<decimal>
            (
                result: 0,
                errorMessage: "No account found with that id."
            );
        }

        if (request.Amount <= 0)
        {
            return new AccountResult<decimal>(
                result: 0, 
                errorMessage: "Please enter valid decimal withdrawal amount greater than zero."
            );
        }
        
        if (request.Amount > foundAccount.Balance)
        {
            return new AccountResult<decimal>(
                result: 0, 
                errorMessage: "Insufficient funds."
            );
        }
        
        foundAccount.Balance += request.Amount;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<decimal>(
                result: 0, 
                errorMessage: ex.Message
            );
        }
        
        return new AccountResult<decimal>(result: foundAccount.Balance);
    }
}