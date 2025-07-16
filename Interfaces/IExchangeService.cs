namespace BankRestApi.Interfaces;

public interface IExchangeService
{
    Task<Dictionary<string, decimal>?> GetExchangeRatesAsync(string currencies);
}