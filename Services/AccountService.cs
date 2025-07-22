using System.Net;
using BankRestApi.ExtensionMethods;
using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
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

    public async Task<AccountResult<IEnumerable<Account>>> GetAccounts(
        string? name,
        string sort,
        bool desc,
        int pageNumber = 1,
        int pageSize = 5)
    {
        var (accounts, paginationMetadata)
            = await _repository.GetAccounts(name, sort, desc, pageNumber, pageSize);
        var result = accounts.Select(a => a.ToDto());
        return new AccountResult<IEnumerable<Account>>(
            HttpStatusCode.OK, 
            (accounts.Select(a => a.ToDto()), paginationMetadata));
    }

    public async Task<AccountResult<Account>> Create(CreateAccount request)
    {
        var name = request.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            return AccountResult<Account>.EmptyNameError();
        }
        var addedAccount = await _repository.Insert(name);
        
        return addedAccount is null
            ? AccountResult<Account>.InternalServerError()
            : addedAccount.CreateResult();
    }

    public async Task<AccountResult<Account>> Get(GetAccount request)
    {
        var foundAccount = await _repository.GetById(request.Id);

        return foundAccount is null
            ? AccountResult<Account>.NotFoundError()
            : foundAccount.CreateResult();
    }
    
    public async Task<AccountResult<Account>> Deposit(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }
        
        var foundAccount = await _repository.GetById(request.Id);
        if (foundAccount is null)
        {
            return AccountResult<Account>.NotFoundError();
        }
        var updatedAccount = await _repository.Update(foundAccount, request.Amount);

        return updatedAccount is null
            ? AccountResult<Account>.InternalServerError()
            : updatedAccount.CreateResult();
    }
    
    public async Task<AccountResult<Account>> Withdraw(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return AccountResult<Account>.NonpositiveAmountError();
        }
        
        var foundAccount = await _repository.GetById(request.Id);
        if (foundAccount is null)
        {
            return AccountResult<Account>.NotFoundError();
        }
        
        if (request.Amount > foundAccount.Balance)
        {
            return AccountResult<Account>.InsufficientFundsError();
        }
        var updatedAccount = await _repository.Update(foundAccount, -1 * request.Amount);

        return updatedAccount is null
            ? AccountResult<Account>.InternalServerError()
            : updatedAccount.CreateResult();
    }
    
    public async Task<AccountResult<TransferDetails>> Transfer(Transaction request)
    {
        var (amount,
            senderId,
            recipientId) = request;
        if (senderId == recipientId)
        {
            return AccountResult<TransferDetails>.DuplicateIdError();
        }
        if (amount <= 0)
        {
            return AccountResult<TransferDetails>.NonpositiveAmountError();
        }
        
        var sender = await _repository.GetById(senderId);
        var recipient = await _repository.GetById(recipientId);
        if (sender is null || recipient is null)
        {
            return AccountResult<TransferDetails>.NotFoundError();
        }
        if (amount > sender.Balance)
        {
            return AccountResult<TransferDetails>.InsufficientFundsError();
        }
        
        var updatedSender = await _repository.Update(sender, -1 * request.Amount);
        var updatedRecipient = await _repository.Update(recipient, request.Amount);

        return updatedSender is null || updatedRecipient is null
            ? AccountResult<TransferDetails>.InternalServerError()
            : new AccountResult<TransferDetails>(
                HttpStatusCode.OK,
                new TransferDetails(sender.ToDto(), recipient.ToDto()));
    }
    
    
    public async Task<AccountResult<ConvertedBalances>> ConvertBalances(ConvertCommand command)
    {
        var foundAccount = await _repository.GetById(command.Id);
        if (foundAccount is null)
        {
            return AccountResult<ConvertedBalances>.NotFoundError();
        }

        var exchangeRateResult = await _exchangeService.GetExchangeRatesAsync(
            string.Join(',', command.Currencies));
        if (exchangeRateResult.ErrorMessage != string.Empty)
        {
            return new AccountResult<ConvertedBalances>(
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
    
        return new AccountResult<ConvertedBalances>(HttpStatusCode.OK, convertedBalances);
    }
}