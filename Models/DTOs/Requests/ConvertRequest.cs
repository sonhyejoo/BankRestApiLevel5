using System.ComponentModel.DataAnnotations;

namespace BankRestApi.Models.DTOs.Requests;

public record ConvertRequest(List<string> Currencies);