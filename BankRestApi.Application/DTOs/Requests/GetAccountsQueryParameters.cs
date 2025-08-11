namespace BankRestApi.Application.DTOs.Requests;

public record GetAccountsQueryParameters(
    string? Name,
    string SortBy = "",
    bool Desc = false,
    int PageNumber = 1,
    int PageSize = 5);