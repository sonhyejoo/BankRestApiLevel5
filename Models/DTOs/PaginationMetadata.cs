namespace BankRestApi.Models.DTOs;

public record PaginationMetadata(
    int TotalItemCount,
    int PageSize,
    int PageNumber);