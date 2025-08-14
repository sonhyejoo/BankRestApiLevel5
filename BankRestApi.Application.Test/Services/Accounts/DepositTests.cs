using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services.Accounts;

public class DepositTests
{
    private IAccountRepository _accountRepository;
    private IExchangeService _exchangeService;

    public DepositTests()
    {
        _accountRepository = new FakeAccountRepository();
        _exchangeService = new FakeExchangeService();
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

    private AccountService CreateDefaultAccountService() => new(_accountRepository, _exchangeService);
}