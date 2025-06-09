using Intellishelf.Models;
using Intellishelf.Models.Auth;
using Intellishelf.Models.Books;

namespace Intellishelf.Services;

public interface IIntellishelfApiClient
{
    Task<AuthResult> LoginAsync(UserCredentials userCredentials);
    Task AddBook(Book book);
    Task<PagedResult<Book>> GetBooksPagedAsync(int page, int pageSize, BookOrderBy orderBy = BookOrderBy.Added, bool ascending = true);
    Task<Book> ParseBookFromTextAsync(string text);
    Task<byte[]> GetImageContentAsync(string userId, string fileName);
}