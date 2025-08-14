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
        
    }

    [Fact]
    public void Get_InvalidId_NotFound()
    {
        
    }

    
    [Fact]
    public void Deposit_ValidRequest_UpdatedAccount()
    {
        
    }

    [Fact]
    public void Deposit_NonpositiveAmount_BadRequest()
    {
        
    }

    [Fact]
    public void Deposit_InvalidId_NotFound()
    {
        
    }

    
    [Fact]
    public void Withdraw_ValidRequest_UpdatedAccount()
    {
        
    }

    [Fact]
    public void Withdraw_NonpositiveAmount_BadRequest()
    {
        
    }

    [Fact]
    public void Withdraw_InvalidId_NotFound()
    {
        
    }

    [Fact]
    public void Withdraw_AmountGreaterThanBalance_InsufficientFunds()
    {
        
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