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
                result: Account.Empty,
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
            return AccountResult<Account>.NotFoundError(Account.Empty);
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
            return AccountResult<decimal>.NotFoundError(0);
        }

        if (request.Amount <= 0)
        {
            return AccountResult<decimal>.GreaterThanZeroError(0);
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
            return AccountResult<decimal>.NotFoundError(0);
        }

        if (request.Amount <= 0)
        {
            return AccountResult<decimal>.GreaterThanZeroError(0);
        }
        
        if (request.Amount > foundAccount.Balance)
        {
            return new AccountResult<decimal>(
                result: 0, 
                errorMessage: "Insufficient funds."
            );
        }
        
        foundAccount.Balance -= request.Amount;
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
    
    public async Task<AccountResult<TransferBalances>> Transfer(TransactionRequest request)
    {
        var (amount, senderId, recipientId) = request;

        if (senderId == recipientId)
        {
            return new AccountResult<TransferBalances>
            (
                result: new TransferBalances(0, 0),
                errorMessage: "Duplicate ids given for sender and recipient."
            );
        }
        
        var sender = await  _context.Accounts.FindAsync(senderId);
        var recipient = await  _context.Accounts.FindAsync(recipientId);

        if (sender == null  || recipient == null)
        {
            return AccountResult<TransferBalances>.NotFoundError(new TransferBalances(0, 0));
        }

        if (amount <= 0)
        {
            return AccountResult<TransferBalances>.GreaterThanZeroError(new TransferBalances(0, 0));
        }
        
        if (amount > sender.Balance)
        {
            return new AccountResult<TransferBalances>(
                result: new TransferBalances(0, 0), 
                errorMessage: "Insufficient funds."
            );
        }
        
        sender.Balance -= request.Amount;
        recipient.Balance +=  request.Amount;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<TransferBalances>(
                result: new TransferBalances(0, 0), 
                errorMessage: ex.Message
            );
        }
        
        return new AccountResult<TransferBalances>(result: new TransferBalances(sender.Balance, recipient.Balance));
    }
}