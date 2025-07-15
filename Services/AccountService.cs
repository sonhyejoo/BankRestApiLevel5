using System.Net;
using BankRestApi.ExtensionMethods;
using BankRestApi.Interfaces;
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
        var (insertSuccess, addedAccount) = await _repository.TryInsert(accountToAdd);
        
        return insertSuccess ? addedAccount.CreateResult() : AccountResult<Account>.InternalServerError();
    }

    public async Task<AccountResult<Account>> Get(GetAccount request)
    {
        var (getSuccess, foundAccount) = await _repository.TryGetById(request.Id);

        return getSuccess ? foundAccount.CreateResult() : AccountResult<Account>.InternalServerError();
    }
    
    public async Task<AccountResult<Account>> Deposit(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }
        var (getSuccess, foundAccount) = await _repository.TryGetById(request.Id);

        if (!getSuccess)
        {
            return AccountResult<Account>.NotFoundError();
        }

        foundAccount.Balance += request.Amount;
        var (updateSuccess, updatedAccount) = await _repository.TryUpdate(foundAccount);

        return updateSuccess ? updatedAccount.CreateResult() : AccountResult<Account>.InternalServerError();
    }
    
    public async Task<AccountResult<Account>> Withdraw(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }
        var (getSuccess, foundAccount) = await _repository.TryGetById(request.Id);
        
        if (!getSuccess)
        {
            return AccountResult<Account>.NotFoundError();
        }
        
        if (request.Amount > foundAccount.Balance)
        {
            return AccountResult<Account>.InsufficientFundsError();
        }
        foundAccount.Balance -= request.Amount;
        var (updateSuccess, updatedAccount) = await _repository.TryUpdate(foundAccount);

        return updateSuccess ? updatedAccount.CreateResult() : AccountResult<Account>.InternalServerError();
    }
    
    public async Task<AccountResult<TransferDetails>> Transfer(Transaction request)
    {
        var (amount, senderId, recipientId) = request;

        if (senderId == recipientId)
        {
            return AccountResult<TransferDetails>.DuplicateIdError();
        }
        
        if (amount <= 0)
        {
            return AccountResult<TransferDetails>.NonpositiveAmountError();
        }
        var (sendGetSuccess, sender) = await  _repository.TryGetById(senderId);
        var (receiveGetSuccess, recipient) = await  _repository.TryGetById(recipientId);

        if (!(sendGetSuccess && receiveGetSuccess))
        {
            return AccountResult<TransferDetails>.NotFoundError();
        }
    
        if (amount > sender.Balance)
        {
            return AccountResult<TransferDetails>.InsufficientFundsError();
        }
        sender.Balance -= request.Amount;
        recipient.Balance +=  request.Amount;
        var (sendUpdated, updatedSender) = await _repository.TryUpdate(sender);
        var (recipientUpdated, updatedRecipient) = await _repository.TryUpdate(recipient);
    
        return sendUpdated && recipientUpdated ? 
            new AccountResult<TransferDetails>(HttpStatusCode.OK, 
                new TransferDetails(sender.ToDto(), recipient.ToDto())) 
            : AccountResult<TransferDetails>.InternalServerError();
    }
    
    
    public async Task<AccountResult<ConvertedBalances>> ConvertBalances(ConvertCommand command)
    {
        var (getSuccess, foundAccount) = await _repository.TryGetById(command.Id);

        if (!getSuccess)
        {
            return AccountResult<ConvertedBalances>.NotFoundError();
        }
        var balanceInUsd = foundAccount.Balance;
        Dictionary<string, decimal> exchangeRates;

        try
        {
            exchangeRates = await _exchangeService.GetExchangeRatesAsync(
                string.Join(',', command.Currencies));
        }
        catch (HttpRequestException ex)
        {
            return new AccountResult<ConvertedBalances>(ex.StatusCode, ex.Message);
        }
        var balances = 
            exchangeRates.ToDictionary(kvp => kvp.Key, kvp => kvp.Value * foundAccount.Balance);

        var convertedBalances =
            new ConvertedBalances(foundAccount.Id, foundAccount.Name, foundAccount.Balance, balances);
        
        return new AccountResult<ConvertedBalances>(HttpStatusCode.OK, convertedBalances);
    }
}