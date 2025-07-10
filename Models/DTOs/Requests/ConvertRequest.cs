using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace BankRestApi.Models.DTOs.Requests;

public record ConvertRequest(
    Guid Id, 
    [AllValidCurrenciesInList]
    List<string> Currencies
    );
