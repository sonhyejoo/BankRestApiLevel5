using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public interface IExchangeService
{
    Task<FreeCurrencyApiResponse?>? GetExchangeRatesAsync(string currencies);
}