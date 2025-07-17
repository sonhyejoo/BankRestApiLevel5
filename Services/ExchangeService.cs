using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public class ExchangeService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public ExchangeService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }
    public async Task<ExchangeRateResult> GetExchangeRatesAsync(string currencies)
    {
        _httpClient.DefaultRequestHeaders.Add("apikey", _config["apikey"]);
        var response = await _httpClient.GetAsync("?currencies=" + currencies);
        if (!response.IsSuccessStatusCode)
        {
            return new ExchangeRateResult(response.StatusCode, response.ToString());
        }
        
        var jsonContent = await response.Content.ReadFromJsonAsync<CurrencyApiResponse>();
        return new ExchangeRateResult(response.StatusCode, jsonContent.Data);
    }
}