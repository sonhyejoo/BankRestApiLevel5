using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services.Accounts;

public class GetAccountsTests
{
    private IAccountRepository _accountRepository;
    private IExchangeService _exchangeService;

    public GetAccountsTests()
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
    
    private AccountService CreateDefaultAccountService() => new(_accountRepository, _exchangeService);
}