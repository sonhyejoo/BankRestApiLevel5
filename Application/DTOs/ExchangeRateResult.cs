using System.Net;

namespace BankRestApi.Models.DTOs;

public record ExchangeRateResult(
    HttpStatusCode StatusCode,
    Dictionary<string, decimal>? ExchangeRates,
    string ErrorMessage = "")
{
    public ExchangeRateResult(
        HttpStatusCode StatusCode,
        string ErrorMessage)
        : this(StatusCode, null, ErrorMessage) { }
}