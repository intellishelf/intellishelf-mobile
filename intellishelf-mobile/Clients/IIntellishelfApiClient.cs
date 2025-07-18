using Intellishelf.Models;
using Intellishelf.Models.Books;

namespace Intellishelf.Clients;

public interface IIntellishelfApiClient
{
    Task AddBook(Book book);
    Task<PagedResult<Book>> GetBooksPagedAsync(int page, int pageSize, BookOrderBy orderBy = BookOrderBy.Added, bool ascending = true);
    Task<Book> ParseBookFromTextAsync(string text);
}