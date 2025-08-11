using System.Net;
using System.Net.Http.Json;
using BankRestApi.Application.DTOs.Results;
using BankRestApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BankRestApi.Application.Services;

public class ExchangeService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public ExchangeService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }
    public async Task<ExchangeRateResult> GetExchangeRates(string currencies)
    {
        _httpClient.DefaultRequestHeaders.Add("apikey", _config["apikey"]);
        var response = await _httpClient.GetAsync("?currencies=" + currencies);
        if (response is { StatusCode: HttpStatusCode.UnprocessableEntity})
        {
            return new ExchangeRateResult(response.StatusCode, "Invalid currencies inputted.");
        }
        if (!response.IsSuccessStatusCode)
        {
            return new ExchangeRateResult(response.StatusCode, response.ReasonPhrase!);
        }
        
        var jsonContent = await response.Content.ReadFromJsonAsync<CurrencyApiResponse>();
        
        return new ExchangeRateResult(response.StatusCode, jsonContent.Data);
    }
}