namespace BankRestApi.Application.DTOs.Results;

public record PaginationMetadata(
    int TotalItemCount,
    int PageSize,
    int PageNumber);