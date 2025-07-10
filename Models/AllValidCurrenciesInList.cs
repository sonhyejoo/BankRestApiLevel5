using System.ComponentModel.DataAnnotations;

namespace BankRestApi.Models;

public class AllValidCurrenciesInList : ValidationAttribute
{
    private static List<string> _currencies = ["EUR", "USD", "JPY", "BGN", "CZK", "DKK", "GBP", "HUF", "PLN", "RON", 
        "SEK", "CHF", "ISK", "NOK", "HRK", "RUB", "TRY", "AUD", "BRL", "CAD", "CNY", "HKD", "IDR", "ILS", "INR", "KRW",
        "MXN", "MYR", "NZD", "PHP", "SGD", "THB", "ZAR"];
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var strings = value as List<string>;
        if (strings is null || (strings.Count == 1 && strings.Contains(string.Empty)))
        {
            return ValidationResult.Success;
        }

        var invalid = strings.Where(s => !_currencies.Contains(s)).ToArray();
        if(invalid.Length > 0)
            return new ValidationResult("The following currencies are invalid or unavailable: " + string.Join(", ", invalid));

        return ValidationResult.Success;
    }
}