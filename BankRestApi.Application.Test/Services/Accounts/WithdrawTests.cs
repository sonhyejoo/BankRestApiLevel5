using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services.Accounts;

public class WithdrawTests
{
    private IAccountRepository _accountRepository;
    private IExchangeService _exchangeService;

    public WithdrawTests()
    {
        _accountRepository = new FakeAccountRepository();
        _exchangeService = new FakeExchangeService();
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

    private AccountService CreateDefaultAccountService() => new(_accountRepository, _exchangeService);
}