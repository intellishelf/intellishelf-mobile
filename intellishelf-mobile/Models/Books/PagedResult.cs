namespace Intellishelf.Models.Books;

public class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalPages { get; init; }
}