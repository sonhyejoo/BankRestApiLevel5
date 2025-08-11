using BankRestApi.Application.DTOs.Results;

namespace BankRestApi.Application.Interfaces;

public interface IExchangeService
{
    Task<ExchangeRateResult> GetExchangeRates(string currencies);
}