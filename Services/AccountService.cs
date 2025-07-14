using System.Net;
using System.Text;
using BankRestApi.ExtensionMethods;
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
        
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            return AccountResult<Account>.EmptyNameError();
        }
        var accountToAdd = new Models.Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            Balance = 0
        };
        var addedAccount = await _repository.Insert(accountToAdd);

        return addedAccount.CreateResult();
    }

    public async Task<AccountResult<Account>> Get(GetAccount request)
    {
        var foundAccount = await _repository.GetById(request.Id);

        return foundAccount is null ? AccountResult<Account>.NotFoundError() : foundAccount.CreateResult();
    }
    
    public async Task<AccountResult<Account>> Deposit(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }
        var foundAccount = await  _repository.GetById(request.Id);
        
        if (foundAccount is null)
        {
            return AccountResult<Account>.NotFoundError();
        }
        foundAccount.Balance += request.Amount;
        try
        {
            await _repository.Update(foundAccount);
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<Account>(HttpStatusCode.InternalServerError, ex.Message);
        }
        
        return foundAccount.CreateResult();
    }
    
    public async Task<AccountResult<Account>> Withdraw(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }
        var foundAccount = await  _repository.GetById(request.Id);

        if (foundAccount is null)
        {
            return AccountResult<Account>.NotFoundError();
        }
        
        if (request.Amount > foundAccount.Balance)
        {
            return AccountResult<Account>.InsufficientFundsError();
        }
        foundAccount.Balance -= request.Amount;
        try
        {
            await _repository.Update(foundAccount);
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<Account>(HttpStatusCode.InternalServerError, ex.Message);
        }
        
        return foundAccount.CreateResult();
    }
    
    public async Task<AccountResult<TransferDetails>> Transfer(Transaction request)
    {
        var (amount, senderId, recipientId) = request;

        if (senderId == recipientId)
        {
            return AccountResult<TransferDetails>.DuplicateIdError();
        }
        var sender = await  _repository.GetById(senderId);
        var recipient = await  _repository.GetById(recipientId);

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
        try
        {
            await _repository.Update(sender);
            await _repository.Update(recipient);
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            return new AccountResult<TransferDetails>(HttpStatusCode.InternalServerError, ex.Message);
        }
        
        return new AccountResult<TransferDetails>(HttpStatusCode.OK, new TransferDetails(sender.ToDto(), recipient.ToDto()));
    }
    
    public async Task<AccountResult<ConvertedBalances>> ConvertBalances(ConvertCommand command)
    {
        var foundAccount = await _repository.GetById(command.Id);

        if (foundAccount is null)
        {
            return AccountResult<ConvertedBalances>.NotFoundError();
        }
        var balanceInUsd = foundAccount.Balance;
        FreeCurrencyApiResponse? freeCurrencyApiResponse;
        try
        {
            freeCurrencyApiResponse = await _exchangeService.GetExchangeRatesAsync(
                string.Join(',', command.Currencies))!;
        }
        catch (HttpRequestException ex)
        {
            return new AccountResult<ConvertedBalances>(ex.StatusCode, ex.Message);
        }
        var exchangeRates = freeCurrencyApiResponse.data;
        var balances = new Dictionary<string, decimal>();
        foreach (var (currency, rate) in exchangeRates)
        {
            balances.Add(currency, balanceInUsd * rate);
        }
        
        return new AccountResult<ConvertedBalances>(HttpStatusCode.OK, new ConvertedBalances(foundAccount.Id, foundAccount.Name, foundAccount.Balance, balances));
    }
}