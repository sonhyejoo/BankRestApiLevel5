namespace BankRestApi.Application.DTOs.Results;

public record PagedAccountsDtoResult(
    IEnumerable<Account> Accounts,
    PaginationMetadata PageData);