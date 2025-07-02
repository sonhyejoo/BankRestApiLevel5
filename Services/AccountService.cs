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
            return AccountResult<Account>.EmptyNameError(Account.Empty);
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
            return AccountResult<Account>.InvalidIdError(Account.Empty);
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
    
    public async Task<AccountResult<Account>> Deposit(TransactionRequest request)
    {
        var foundAccount = await  _context.Accounts.FindAsync(request.Id);

        if (foundAccount == null)
        {
            return AccountResult<Account>.InvalidIdError(Account.Empty);
        }

        if (request.Amount <= 0)
        {
            return AccountResult<Account>.GreaterThanZeroError(Account.Empty);
        }
        
        foundAccount.Balance += request.Amount;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<Account>(
                result: Account.Empty, 
                errorMessage: ex.Message
            );
        }
        
        return new AccountResult<Account>(new Account
        {
            Id = foundAccount.Id,
            Name = foundAccount.Name,
            Balance = foundAccount.Balance
        });
    }
    
    public async Task<AccountResult<Account>> Withdraw(TransactionRequest request)
    {
        var foundAccount = await  _context.Accounts.FindAsync(request.Id);

        if (foundAccount == null)
        {
            return AccountResult<Account>.InvalidIdError(Account.Empty);
        }

        if (request.Amount <= 0)
        {
            return AccountResult<Account>.GreaterThanZeroError(Account.Empty);
        }
        
        if (request.Amount > foundAccount.Balance)
        {
            return AccountResult<Account>.InsufficientFundsError(Account.Empty);
        }
        
        foundAccount.Balance -= request.Amount;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<Account>(
                result: Account.Empty, 
                errorMessage: ex.Message
            );
        }
        
        return new AccountResult<Account> (new Account
        {
            Id = foundAccount.Id,
            Name = foundAccount.Name,
            Balance = foundAccount.Balance
        });
    }
    
    public async Task<AccountResult<TransferDetails>> Transfer(TransactionRequest request)
    {
        var (amount, senderId, recipientId) = request;

        if (senderId == recipientId)
        {
            return AccountResult<TransferDetails>.DuplicateIdError(TransferDetails.Empty);
        }
        
        var sender = await  _context.Accounts.FindAsync(senderId);
        var recipient = await  _context.Accounts.FindAsync(recipientId);

        if (sender == null  || recipient == null)
        {
            return AccountResult<TransferDetails>.InvalidIdError(TransferDetails.Empty);
        }

        if (amount <= 0)
        {
            return AccountResult<TransferDetails>.GreaterThanZeroError(TransferDetails.Empty);
        }
        
        if (amount > sender.Balance)
        {
            return AccountResult<TransferDetails>.InsufficientFundsError(TransferDetails.Empty);
        }
        
        sender.Balance -= request.Amount;
        recipient.Balance +=  request.Amount;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<TransferDetails>(
                result: TransferDetails.Empty, 
                errorMessage: ex.Message
            );
        }
        
        return new AccountResult<TransferDetails>(result: new TransferDetails(sender.Balance, recipient.Balance));
    }
}