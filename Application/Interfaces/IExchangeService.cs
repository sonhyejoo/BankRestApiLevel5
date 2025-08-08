using Application.DTOs.Accounts.Results;

namespace Application.Interfaces;

public interface IExchangeService
{
    Task<ExchangeRateResult> GetExchangeRates(string currencies);
}