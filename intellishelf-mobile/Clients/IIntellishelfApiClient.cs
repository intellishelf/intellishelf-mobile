using Intellishelf.Common.TryResult;
using Intellishelf.Models.Books;

namespace Intellishelf.Clients;

public interface IIntellishelfApiClient
{
    Task<TryResult> AddBook(Book book);
    Task<TryResult<PagedResult<Book>>> GetBooksPagedAsync(int page, int pageSize, BookOrderBy orderBy = BookOrderBy.Added, bool ascending = true);
    Task<TryResult<Book>> ParseBookFromTextAsync(string text);
}