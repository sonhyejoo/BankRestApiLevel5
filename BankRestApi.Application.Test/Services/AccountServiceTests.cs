using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;

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
    public void Transfer_ValidRequest_UpdatedAccounts()
    {
        
    }

    [Fact]
    public void Transfer_SameSenderAndRecipient_BadRequest()
    {
        
    }

    [Fact]
    public void Transfer_NonpositiveAmount_BadRequest()
    {
        
    }

    [Fact]
    public void Transfer_InvalidSenderId_NotFound()
    {
        
    }

    [Fact]
    public void Transfer_InvalidRecipientId_NotFound()
    {
        
    }

    [Fact]
    public void Transfer_AmountGreaterThanSenderBalance_BadRequest()
    {
        
    }

    
    [Fact]
    public void ConvertBalances_UnspecifiedCurrencies_AccountWithConvertedBalances()
    {
        
    }

    [Fact]
    public void ConvertBalances_SpecifiedCurrencies_AccountWithSpecifiedConversions()
    {
        
    }

    [Fact]
    public void ConvertBalances_InvalidId_NotFound()
    {
        
    }

    [Fact]
    public void ConvertBalances_InvalidCurrencies_BadRequest()
    {
        
    }

    private AccountService CreateDefaultAccountService() => new AccountService(_accountRepository, _exchangeService);
}