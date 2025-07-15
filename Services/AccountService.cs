using System.Net;
using System.Text;
using BankRestApi.ExtensionMethods;
using BankRestApi.Interfaces;
using BankRestApi.Models;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
using Microsoft.EntityFrameworkCore;
using Account = BankRestApi.Models.DTOs.Account;

namespace BankRestApi.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly IExchangeService _exchangeService;
    public AccountService(IAccountRepository accountRepository, IExchangeService exchangeService)
    {
        _repository = accountRepository;
        _exchangeService = exchangeService;
    }

    public async Task<AccountResult<Account>> Create(CreateAccount request)
    {
        var name = request.Name;
        
        if (string.IsNullOrWhiteSpace(name))
        {
            return AccountResult<Account>.EmptyNameError();
        }
        var accountToAdd = new Models.Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            Balance = 0
        };
        try
        {
            var addedAccount = await _repository.TryInsert(accountToAdd);
            
            return addedAccount.CreateResult();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.InternalServerError, ex.Message);
        }

    }

    public async Task<AccountResult<Account>> Get(GetAccount request)
    {
        try
        {
            var foundAccount = await _repository.TryGetById(request.Id);

            return foundAccount.CreateResult();
        }
        catch (KeyNotFoundException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.NotFound, ex.Message);
        }
    }
    
    public async Task<AccountResult<Account>> Deposit(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }

        try
        {
            var foundAccount = await _repository.TryGetById(request.Id);

            if (foundAccount is null)
            {
                return AccountResult<Account>.NotFoundError();
            }

            foundAccount.Balance += request.Amount;
            await _repository.TryUpdate(foundAccount);

            return foundAccount.CreateResult();
        }
        catch (KeyNotFoundException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
    
    public async Task<AccountResult<Account>> Withdraw(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }

        try
        {
            var foundAccount = await _repository.TryGetById(request.Id);

            if (request.Amount > foundAccount.Balance)
            {
                return AccountResult<Account>.InsufficientFundsError();
            }
            foundAccount.Balance -= request.Amount;
            await _repository.TryUpdate(foundAccount);

            return foundAccount.CreateResult();
        }
        catch (KeyNotFoundException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return new AccountResult<Account>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
    
    public async Task<AccountResult<TransferDetails>> Transfer(Transaction request)
    {
        var (amount, senderId, recipientId) = request;

        if (senderId == recipientId)
        {
            return AccountResult<TransferDetails>.DuplicateIdError();
        }

        try
        {
            var sender = await  _repository.TryGetById(senderId);
            var recipient = await  _repository.TryGetById(recipientId);

            if (sender is null  || recipient is null)
            {
                return AccountResult<TransferDetails>.NotFoundError();
            }

            if (amount <= 0)
            {
                return AccountResult<TransferDetails>.NonpositiveAmountError();
            }
        
            if (amount > sender.Balance)
            {
                return AccountResult<TransferDetails>.InsufficientFundsError();
            }
            sender.Balance -= request.Amount;
            recipient.Balance +=  request.Amount;
            await _repository.TryUpdate(sender);
            await _repository.TryUpdate(recipient);
        
            return new AccountResult<TransferDetails>(HttpStatusCode.OK, new TransferDetails(sender.ToDto(), recipient.ToDto()));
        }
        catch (KeyNotFoundException ex)
        {
            return new AccountResult<TransferDetails>(HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return new AccountResult<TransferDetails>(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<TransferDetails>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
    
    public async Task<AccountResult<ConvertedBalances>> ConvertBalances(ConvertCommand command)
    {
        var foundAccount = await _repository.TryGetById(command.Id);

        if (foundAccount is null)
        {
            return AccountResult<ConvertedBalances>.NotFoundError();
        }
        var balanceInUsd = foundAccount.Balance;
        Dictionary<string, decimal> exchangeRates;
        
        try
        {
            exchangeRates = await _exchangeService.GetExchangeRatesAsync(string.Join(',', command.Currencies))!;
        }
        catch (HttpRequestException ex)
        {
            return new AccountResult<ConvertedBalances>(ex.StatusCode, ex.Message);
        }
        var balances = new Dictionary<string, decimal>();
        
        foreach (var (currency, rate) in exchangeRates)
        {
            balances.Add(currency, balanceInUsd * rate);
        }

        var convertedBalances =
            new ConvertedBalances(foundAccount.Id, foundAccount.Name, foundAccount.Balance, balances);
        
        return new AccountResult<ConvertedBalances>(HttpStatusCode.OK, convertedBalances);
    }
}