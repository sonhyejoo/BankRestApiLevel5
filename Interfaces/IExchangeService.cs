using BankRestApi.Models.DTOs;

namespace BankRestApi.Interfaces;

public interface IExchangeService
{
    Task<ExchangeRateResult> GetExchangeRatesAsync(string currencies);
}