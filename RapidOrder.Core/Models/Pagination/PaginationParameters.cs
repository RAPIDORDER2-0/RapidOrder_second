namespace RapidOrder.Core.Models.Pagination;

public sealed record PaginationParameters
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int PageNumber { get; init; } = DefaultPageNumber;
    public int PageSize { get; init; } = DefaultPageSize;

    public PaginationParameters Normalize()
    {
        var pageNumber = PageNumber < 1 ? DefaultPageNumber : PageNumber;
        var pageSize = PageSize < 1 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize);
        return this with { PageNumber = pageNumber, PageSize = pageSize };
    }
}
