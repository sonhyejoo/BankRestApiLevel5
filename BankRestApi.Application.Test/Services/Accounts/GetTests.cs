using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Domain.Entities;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services.Accounts;

public class GetTests
{
    private IAccountRepository _accountRepository;
    private IExchangeService _exchangeService;

    public GetTests()
    {
        _accountRepository = new FakeAccountRepository();
        _exchangeService = new FakeExchangeService();
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
    
    private AccountService CreateDefaultAccountService() => new(_accountRepository, _exchangeService);
}