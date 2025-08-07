namespace BankRestApi.Models.DTOs;

public record PagedAccountsDtoResult(
    IEnumerable<Account> Accounts,
    PaginationMetadata PageData);