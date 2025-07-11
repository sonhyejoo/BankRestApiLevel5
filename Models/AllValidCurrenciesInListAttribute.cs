using System.ComponentModel.DataAnnotations;

namespace BankRestApi.Models;

public class AllValidCurrenciesInListAttribute : ValidationAttribute
{
    private static List<string> _currencies = ["EUR", "USD", "JPY", "BGN", "CZK", "DKK", "GBP", "HUF", "PLN", "RON", 
        "SEK", "CHF", "ISK", "NOK", "HRK", "RUB", "TRY", "AUD", "BRL", "CAD", "CNY", "HKD", "IDR", "ILS", "INR", "KRW",
        "MXN", "MYR", "NZD", "PHP", "SGD", "THB", "ZAR"];
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var strings = value as IEnumerable<string>;
        if (strings is null || (strings.Count() == 1 && strings.First() == ""))
        {
            return ValidationResult.Success;
        }

        var invalidCurrencies = strings.Where(s => !_currencies.Contains(s));
        if (invalidCurrencies.Any())
        {
            return new ValidationResult("The following currencies are invalid or unavailable: " + string.Join(", ", invalidCurrencies));
        }

        return ValidationResult.Success;
    }
}