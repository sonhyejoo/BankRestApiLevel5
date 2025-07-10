using BankRestApi.Models.DTOs;

namespace BankRestApi.Services;

public class ExchangeService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.freecurrencyapi.com/v1/currencies"; 
    
    public ExchangeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string currencies)
    {
        _httpClient.DefaultRequestHeaders.Add("apikey", "placeholder");
        _httpClient.DefaultRequestHeaders.Add("currencies", currencies);
        _httpClient.BaseAddress = new Uri(BaseUrl);
        var response = await _httpClient.GetFromJsonAsync<FreeCurrencyApiResponse>("?currencies=" + currencies);
        return response.data;
    }
}