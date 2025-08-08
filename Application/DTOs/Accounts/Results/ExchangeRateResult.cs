using System.Net;

namespace Application.DTOs.Accounts.Results;

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