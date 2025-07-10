namespace BankRestApi.Services;

public interface IExchangeService
{
    Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string currencies);
}