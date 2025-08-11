namespace BankRestApi.Application.DTOs.Accounts.Results;

public record PagedAccountsDtoResult(
    IEnumerable<Account> Accounts,
    PaginationMetadata PageData);