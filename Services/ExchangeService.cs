using System.Net;
using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public class ExchangeService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<ExchangeService> _logger;

    public ExchangeService(HttpClient httpClient, IConfiguration config, ILogger<ExchangeService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }
    public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string currencies)
    {
        _httpClient.DefaultRequestHeaders.Add("apikey", _config["apikey"]);
        var response = await _httpClient.GetFromJsonAsync<CurrencyApiResponse>("?currencies=" + currencies);

        return response.data;
    }
}