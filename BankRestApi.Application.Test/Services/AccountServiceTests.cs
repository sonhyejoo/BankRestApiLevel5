using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;
using Xunit.Abstractions;

namespace BankRestApi.Application.Test.Services;

public class AccountServiceTests
{
    private IAccountRepository _accountRepository;
    private IExchangeService _exchangeService;

    public AccountServiceTests()
    {
        _accountRepository = new FakeAccountRepository();
        _exchangeService = new FakeExchangeService();
    }
    [Fact]
    public async Task GetAccounts_NoQueryParametersSpecified_DefaultQuery()
    {
        var accountService = CreateDefaultAccountService();
        var queryParameters = new GetAccountsQueryParameters(null);
        
        var result = await accountService.GetAccounts(queryParameters);
        
        Assert.Equivalent(
            new BaseResult<PagedAccountsDtoResult>(
                HttpStatusCode.OK,
                new PagedAccountsDtoResult(
                    new List<DTOs.Account>(),
                    new PaginationMetadata(0, 5, 1))), 
            result);
    }
    
    [Fact]
    public async Task GetAccounts_AllQueryParametersSupplied_CorrectQuery()
    {
        var accountService = CreateDefaultAccountService();
        var queryParameters = new GetAccountsQueryParameters(null, "", true, 2, 3);
        
        var result = await accountService.GetAccounts(queryParameters);
        
        Assert.Equivalent(
            new BaseResult<PagedAccountsDtoResult>(
                HttpStatusCode.OK,
                new PagedAccountsDtoResult(
                    new List<DTOs.Account>(),
                    new PaginationMetadata(0, 3, 2))), 
            result);
    }

    
    [Fact]
    public async Task Create_ValidName_CreatedAccount()
    {
        var accountService = CreateDefaultAccountService();
        var request = new CreateAccount("name");

        var result = await accountService.Create(request);
        
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.IsType(typeof(Guid), result.Result.Id);
        Assert.Equal(request.Name, result.Result.Name);
        Assert.Equal(0, result.Result.Balance);
    }

    [Fact]
    public async Task Create_EmptyName_BadRequest()
    {
        var accountService = CreateDefaultAccountService();
        var request = new CreateAccount("");

        var result = await accountService.Create(request);
        
        Assert.Equivalent(BaseResult<Account>.EmptyNameError(), result);
    }

    
    [Fact]
    public async Task Get_ValidId_Account()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new GetAccount(addedAccount.Id);

        var result = await accountService.Get(request);
        
        Assert.Equivalent(addedAccount.CreateResult(), result);
    }

    [Fact]
    public async Task Get_InvalidId_NotFound()
    {
        var accountService = CreateDefaultAccountService();
        var request = new GetAccount(Guid.NewGuid());

        var result = await accountService.Get(request);
        
        Assert.Equivalent(BaseResult<Account>.NotFoundError(), result);
    }

    
    [Fact]
    public async Task Deposit_ValidRequest_UpdatedAccount()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new Transaction(1, addedAccount.Id);

        var result = await accountService.Deposit(request);
        
        Assert.Equivalent(addedAccount.CreateResult(), result);
        Assert.Equal(1, result.Result.Balance);
    }

    [Fact]
    public async Task Deposit_NonpositiveAmount_BadRequest()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new Transaction(0, addedAccount.Id);

        var result = await accountService.Deposit(request);
        
        Assert.Equivalent(BaseResult<Account>.NonpositiveAmountError(), result);
    }

    [Fact]
    public async Task Deposit_InvalidId_NotFound()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new Transaction(0, addedAccount.Id);

        var result = await accountService.Deposit(request);
        
        Assert.Equivalent(BaseResult<Account>.NonpositiveAmountError(), result);
    }

    
    [Fact]
    public async Task Withdraw_ValidRequest_UpdatedAccount()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        await _accountRepository.Update(addedAccount, 2);
        var request = new Transaction(1, addedAccount.Id);

        var result = await accountService.Withdraw(request);
        
        Assert.Equivalent(addedAccount.CreateResult(), result);
        Assert.Equal(1, result.Result.Balance);
    }

    [Fact]
    public async Task Withdraw_NonpositiveAmount_BadRequest()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var balanceShouldBeOne = await _accountRepository.Update(addedAccount, 1);
        var request = new Transaction(0, addedAccount.Id);

        var result = await accountService.Withdraw(request);
        
        Assert.Equivalent(BaseResult<Account>.NonpositiveAmountError(), result);
        Assert.Equal(1, balanceShouldBeOne.Balance);
    }

    [Fact]
    public async Task Withdraw_InvalidId_NotFound()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var balanceShouldBeTwo = await _accountRepository.Update(addedAccount, 2);
        var request = new Transaction(1, Guid.NewGuid());

        var result = await accountService.Withdraw(request);
        
        Assert.Equivalent(BaseResult<Account>.NotFoundError(), result);
        Assert.Equal(2, balanceShouldBeTwo.Balance);
    }

    [Fact]
    public async Task Withdraw_AmountGreaterThanBalance_InsufficientFunds()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var balanceShouldBeOne = await _accountRepository.Update(addedAccount, 1);
        var request = new Transaction(3, addedAccount.Id);

        var result = await accountService.Withdraw(request);
        
        Assert.Equivalent(BaseResult<Account>.InsufficientFundsError(), result);
        Assert.Equal(1, balanceShouldBeOne.Balance);
    }

    
    [Fact]
    public async Task Transfer_ValidRequest_UpdatedAccounts()
    {
        var accountService = CreateDefaultAccountService();
        var sender = await _accountRepository.Add("sender");
        var recipient = await _accountRepository.Add("recipient");
        var senderBalanceShouldBeZero = await _accountRepository.Update(sender, 1);
        var request = new Transaction(1, sender.Id, recipient.Id);

        var result = await accountService.Transfer(request);
        var recipientBalanceShouldBeOne = await _accountRepository.GetById(recipient.Id);
        
        Assert.Equivalent(
            new BaseResult<TransferDetails>(
                HttpStatusCode.OK,
                new TransferDetails(sender.ToDto(), recipient.ToDto())),
            result);
        Assert.Equal(0, senderBalanceShouldBeZero.Balance);
        Assert.Equal(1, recipientBalanceShouldBeOne.Balance);
    }

    [Fact]
    public async Task Transfer_SameSenderAndRecipient_BadRequest()
    {
        var accountService = CreateDefaultAccountService();
        var sender = await _accountRepository.Add("sender");
        var senderBalanceShouldBeOne = await _accountRepository.Update(sender, 1);
        var request = new Transaction(1, sender.Id, sender.Id);

        var result = await accountService.Transfer(request);
        
        Assert.Equivalent(
            BaseResult<TransferDetails>.DuplicateIdError(),
            result);
        Assert.Equal(1, senderBalanceShouldBeOne.Balance);
    }

    [Fact]
    public async Task Transfer_NonpositiveAmount_BadRequest()
    {
        var accountService = CreateDefaultAccountService();
        var sender = await _accountRepository.Add("sender");
        var recipient = await _accountRepository.Add("recipient");
        var senderBalanceShouldBeOne = await _accountRepository.Update(sender, 1);
        var request = new Transaction(0, sender.Id, recipient.Id);

        var result = await accountService.Transfer(request);
        var recipientBalanceShouldBeZero = await _accountRepository.GetById(recipient.Id);
        
        Assert.Equivalent(
            BaseResult<TransferDetails>.NonpositiveAmountError(),
            result);
        Assert.Equal(1, senderBalanceShouldBeOne.Balance);
        Assert.Equal(0, recipientBalanceShouldBeZero.Balance);
    }

    [Fact]
    public async Task Transfer_InvalidSenderId_NotFound()
    {
        var accountService = CreateDefaultAccountService();
        var sender = await _accountRepository.Add("sender");
        var recipient = await _accountRepository.Add("recipient");
        var senderBalanceShouldBeOne = await _accountRepository.Update(sender, 1);
        var request = new Transaction(1, Guid.NewGuid(), recipient.Id);

        var result = await accountService.Transfer(request);
        var recipientBalanceShouldBeZero = await _accountRepository.GetById(recipient.Id);
        
        Assert.Equivalent(
            BaseResult<TransferDetails>.NotFoundError(),
            result);
        Assert.Equal(1, senderBalanceShouldBeOne.Balance);
        Assert.Equal(0, recipientBalanceShouldBeZero.Balance);
    }

    [Fact]
    public async Task Transfer_InvalidRecipientId_NotFound()
    {
        var accountService = CreateDefaultAccountService();
        var sender = await _accountRepository.Add("sender");
        var recipient = await _accountRepository.Add("recipient");
        var senderBalanceShouldBeOne = await _accountRepository.Update(sender, 1);
        var request = new Transaction(1, sender.Id, Guid.NewGuid());

        var result = await accountService.Transfer(request);
        var recipientBalanceShouldBeZero = await _accountRepository.GetById(recipient.Id);
        
        Assert.Equivalent(
            BaseResult<TransferDetails>.NotFoundError(),
            result);
        Assert.Equal(1, senderBalanceShouldBeOne.Balance);
        Assert.Equal(0, recipientBalanceShouldBeZero.Balance);
    }

    [Fact]
    public async Task Transfer_AmountGreaterThanSenderBalance_BadRequest()
    {
        var accountService = CreateDefaultAccountService();
        var sender = await _accountRepository.Add("sender");
        var recipient = await _accountRepository.Add("recipient");
        var senderBalanceShouldBeOne = await _accountRepository.Update(sender, 1);
        var request = new Transaction(2, sender.Id, recipient.Id);

        var result = await accountService.Transfer(request);
        var recipientBalanceShouldBeZero = await _accountRepository.GetById(recipient.Id);
        
        Assert.Equivalent(
            BaseResult<TransferDetails>.InsufficientFundsError(),
            result);
        Assert.Equal(1, senderBalanceShouldBeOne.Balance);
        Assert.Equal(0, recipientBalanceShouldBeZero.Balance);
    }

    
    [Fact]
    public async Task ConvertBalances_UnspecifiedCurrencies_AccountWithConvertedBalances()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new ConvertCommand(addedAccount.Id, []);
        var convertedBalances = await _exchangeService.GetExchangeRates("");

        var result = await accountService.ConvertBalances(request);
        
        Assert.Equivalent(
            new BaseResult<ConvertedBalances>(
                HttpStatusCode.OK,
                new ConvertedBalances(
                    addedAccount.Id,
                    addedAccount.Name,
                    addedAccount.Balance,
                    convertedBalances.ExchangeRates)),
            result);
    }

    [Fact]
    public async Task ConvertBalances_SpecifiedCurrencies_AccountWithSpecifiedConversions()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new ConvertCommand(addedAccount.Id, ["EUR", "CAD"]);
        var convertedBalances = await _exchangeService.GetExchangeRates("EUR,CAD");

        var result = await accountService.ConvertBalances(request);
        
        Assert.Equivalent(
            new BaseResult<ConvertedBalances>(
                HttpStatusCode.OK,
                new ConvertedBalances(
                    addedAccount.Id,
                    addedAccount.Name,
                    addedAccount.Balance,
                    convertedBalances.ExchangeRates)),
            result);
        Assert.Equal(2, result.Result.Balances.Count);
    }

    [Fact]
    public async Task ConvertBalances_InvalidId_NotFound()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new ConvertCommand(Guid.NewGuid(), ["EUR", "CAD"]);
        var convertedBalances = await _exchangeService.GetExchangeRates("EUR,CAD");

        var result = await accountService.ConvertBalances(request);
        
        Assert.Equivalent(
            BaseResult<ConvertedBalances>.NotFoundError(),
            result);
    }

    [Fact]
    public async Task ConvertBalances_InvalidCurrencies_BadRequest()
    {
        var accountService = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new ConvertCommand(addedAccount.Id, ["EUR", "Asdf"]);
        var result = await accountService.ConvertBalances(request);
        
        Assert.Equivalent(
            new BaseResult<ConvertedBalances>(
                HttpStatusCode.UnprocessableEntity,
                "Invalid currencies inputted."),
            result);
    }

    private AccountService CreateDefaultAccountService() => new AccountService(_accountRepository, _exchangeService);
}