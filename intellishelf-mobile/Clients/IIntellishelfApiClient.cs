using Intellishelf.Models;

namespace Intellishelf.Clients;

public interface IIntellishelfApiClient
{
    Task<AuthToken> LoginAsync(UserCredentials userCredentials);
    Task AddBook(Book book);
    Task<IEnumerable<Book>> GetBooksAsync();
    Task<PagedResult<Book>> GetBooksPagedAsync(int page, int pageSize, BookOrderBy orderBy = BookOrderBy.Added, bool ascending = true);
    Task<Book> ParseBookFromTextAsync(string text);
}
