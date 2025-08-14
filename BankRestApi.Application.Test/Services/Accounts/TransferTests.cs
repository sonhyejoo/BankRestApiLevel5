using System.Net;
using BankRestApi.Application.DTOs.Requests;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using BankRestApi.Application.Services;
using BankRestApi.Infrastructure.Fake;

namespace BankRestApi.Application.Test.Services.Accounts;

public class TransferTests
{
    private IAccountRepository _accountRepository;
    private IExchangeService _exchangeService;

    public TransferTests()
    {
        _accountRepository = new FakeAccountRepository();
        _exchangeService = new FakeExchangeService();
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

    private AccountService CreateDefaultAccountService() => new(_accountRepository, _exchangeService);
}