using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services.Accounts;

public class CreateTests
{
    private IAccountRepository _accountRepository;
    
    private IExchangeService _exchangeService;

    public CreateTests()
    {
        _accountRepository = new FakeAccountRepository();
        _exchangeService = new FakeExchangeService();
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
    
    private AccountService CreateDefaultAccountService() => new AccountService(_accountRepository, _exchangeService);
}