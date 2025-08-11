namespace BankRestApi.Application.DTOs.Accounts.Results;

public record PaginationMetadata(
    int TotalItemCount,
    int PageSize,
    int PageNumber);