﻿using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using Account = BankRestApi.Application.DTOs.Account;

namespace BankRestApi.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly IExchangeService _exchangeService;
    public AccountService(IAccountRepository accountRepository, IExchangeService exchangeService)
    {
        _repository = accountRepository;
        _exchangeService = exchangeService;
    }

    public async Task<BaseResult<PagedAccountsDtoResult>> GetAccounts(GetAccountsQueryParameters queryParameters)
    {
        var (accounts, pageData) = 
            await _repository.GetAccounts(queryParameters);
        var accountDtoList = accounts.Select(a => a.ToDto());
        var result = new PagedAccountsDtoResult(accountDtoList, pageData);
        
        return new BaseResult<PagedAccountsDtoResult>(HttpStatusCode.OK, result);
    }

    public async Task<BaseResult<Account>> Create(CreateAccount request)
    {
        var name = request.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            return BaseResult<Account>.EmptyNameError();
        }
        var addedAccount = await _repository.Add(name);
        
        return addedAccount.CreateResult();
    }

    public async Task<BaseResult<Account>> Get(GetAccount request)
    {
        var foundAccount = await _repository.GetById(request.Id);

        return foundAccount is null
            ? BaseResult<Account>.NotFoundError()
            : foundAccount.CreateResult();
    }
    
    public async Task<BaseResult<Account>> Deposit(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return BaseResult<Account>.NonpositiveAmountError();
        }
        
        var foundAccount = await _repository.GetById(request.Id);
        if (foundAccount is null)
        {
            return BaseResult<Account>.NotFoundError();
        }
        var updatedAccount = await _repository.Update(foundAccount, request.Amount);

        return updatedAccount.CreateResult();
    }
    
    public async Task<BaseResult<Account>> Withdraw(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return BaseResult<Account>.NonpositiveAmountError();
        }
        
        var foundAccount = await _repository.GetById(request.Id);
        if (foundAccount is null)
        {
            return BaseResult<Account>.NotFoundError();
        }
        
        if (request.Amount > foundAccount.Balance)
        {
            return BaseResult<Account>.InsufficientFundsError();
        }
        var updatedAccount = await _repository.Update(foundAccount, -1 * request.Amount);

        return updatedAccount.CreateResult();
    }
    
    public async Task<BaseResult<TransferDetails>> Transfer(Transaction request)
    {
        var (amount,
            senderId,
            recipientId) = request;
        if (senderId == recipientId)
        {
            return BaseResult<TransferDetails>.DuplicateIdError();
        }
        if (amount <= 0)
        {
            return BaseResult<TransferDetails>.NonpositiveAmountError();
        }
        
        var sender = await _repository.GetById(senderId);
        var recipient = await _repository.GetById(recipientId);
        if (sender is null || recipient is null)
        {
            return BaseResult<TransferDetails>.NotFoundError();
        }
        if (amount > sender.Balance)
        {
            return BaseResult<TransferDetails>.InsufficientFundsError();
        }
        
        var updatedSender = await _repository.Update(sender, -1 * request.Amount);
        var updatedRecipient = await _repository.Update(recipient, request.Amount);

        return new BaseResult<TransferDetails>(
                HttpStatusCode.OK,
                new TransferDetails(sender.ToDto(), recipient.ToDto()));
    }
    
    public async Task<BaseResult<ConvertedBalances>> ConvertBalances(ConvertCommand command)
    {
        var foundAccount = await _repository.GetById(command.Id);
        if (foundAccount is null)
        {
            return BaseResult<ConvertedBalances>.NotFoundError();
        }

        var exchangeRateResult = await _exchangeService.GetExchangeRates(
            string.Join(',', command.Currencies));
        if (exchangeRateResult.ErrorMessage != string.Empty)
        {
            return new BaseResult<ConvertedBalances>(
                exchangeRateResult.StatusCode,
                exchangeRateResult.ErrorMessage);
        }
        var balances = exchangeRateResult.ExchangeRates.ToDictionary(
            currencyRate => currencyRate.Key, 
            currencyRate => currencyRate.Value * foundAccount.Balance);
        var convertedBalances = new ConvertedBalances(
            foundAccount.Id,
            foundAccount.Name,
            foundAccount.Balance,
            balances);
    
        return new BaseResult<ConvertedBalances>(HttpStatusCode.OK, convertedBalances);
    }
}