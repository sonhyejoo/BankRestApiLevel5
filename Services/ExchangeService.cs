using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public class ExchangeService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<IExchangeService> _logger;

    public ExchangeService(HttpClient httpClient, IConfiguration config, ILogger<IExchangeService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }
    public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string currencies)
    {
        _httpClient.DefaultRequestHeaders.Add("apikey", _config["apikey"]);
        var response = await _httpClient.GetFromJsonAsync<FreeCurrencyApiResponse>("?currencies=" + currencies);
        _logger.LogInformation("Response: " + response);
        return response.data;
    }
}