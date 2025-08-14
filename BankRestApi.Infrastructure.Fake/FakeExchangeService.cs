using System.Net;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;

namespace BankRestApi.Infrastructure.Fake;

public class FakeExchangeService : IExchangeService
{
    private List<string> _validCurrencies =
    [
        "EUR",
        "USD",
        "JPY",
        "BGN",
        "CZK",
        "DKK",
        "GBP",
        "HUF",
        "PLN",
        "RON",
        "SEK",
        "CHF",
        "ISK",
        "NOK",
        "HRK",
        "RUB",
        "TRY",
        "AUD",
        "BRL",
        "CAD",
        "CNY",
        "HKD",
        "IDR",
        "ILS",
        "INR",
        "KRW",
        "MXN",
        "MYR",
        "NZD",
        "PHP",
        "SGD",
        "THB",
        "ZAR"
    ];

    public Task<ExchangeRateResult> GetExchangeRates(string currencies)
    {
        var splitCurrencies = currencies.Split(",");
        if (currencies is not ""
            && splitCurrencies.Any(currency 
                => !_validCurrencies.Contains(currency) 
                   || string.IsNullOrWhiteSpace(currency)))
        {
            return Task.FromResult(new ExchangeRateResult(HttpStatusCode.UnprocessableEntity, "Invalid currencies inputted."));
        }

        Dictionary<string, decimal> result;
        if (splitCurrencies.Length > 0)
        {
            result = splitCurrencies.ToDictionary(
                currency => currency,
                _ => (decimal) 0);
        }
        else
        {
            result = _validCurrencies.ToDictionary(currency => currency, _ => (decimal)0);
        }

        return Task.FromResult(new ExchangeRateResult(HttpStatusCode.OK, result));
    }
}