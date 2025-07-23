namespace BankRestApi.Models.DTOs;

public record AccountsAndPageData(
    IEnumerable<Account> Accounts,
    PaginationMetadata PageData);