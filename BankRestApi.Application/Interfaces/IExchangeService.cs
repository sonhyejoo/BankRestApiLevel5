using BankRestApi.Application.DTOs.Accounts.Results;

namespace BankRestApi.Application.Interfaces;

public interface IExchangeService
{
    Task<ExchangeRateResult> GetExchangeRates(string currencies);
}