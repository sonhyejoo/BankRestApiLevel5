using BankRestApi.Models.DTOs;

namespace BankRestApi.Interfaces;

public interface IExchangeService
{
    Task<ExchangeRateResult> GetExchangeRates(string currencies);
}