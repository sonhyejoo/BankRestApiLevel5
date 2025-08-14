using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services.Accounts;

public class ConvertBalancesTests
{
    private IAccountRepository _accountRepository;
    private IExchangeService _exchangeService;

    public ConvertBalancesTests()
    {
        _accountRepository = new FakeAccountRepository();
        _exchangeService = new FakeExchangeService();
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
        var request = new ConvertCommand(Guid.NewGuid(), ["EUR", "CAD"]);

        var result = await accountService.ConvertBalances(request);
        
        Assert.Equivalent(
            BaseResult<ConvertedBalances>.NotFoundError(),
            result);
    }

    [Fact]
    public async Task ConvertBalances_InvalidCurrencies_BadRequest()
    {
        var systemUnderTest = CreateDefaultAccountService();
        var addedAccount = await _accountRepository.Add("name");
        var request = new ConvertCommand(addedAccount.Id, ["EUR", "Asdf"]);
        var result = await systemUnderTest.ConvertBalances(request);
        
        Assert.Equivalent(
            new BaseResult<ConvertedBalances>(
                HttpStatusCode.UnprocessableEntity,
                "Invalid currencies inputted."),
            result);
    }

    private AccountService CreateDefaultAccountService() => new(_accountRepository, _exchangeService);
}