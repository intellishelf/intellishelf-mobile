using Intellishelf.Models;

namespace Intellishelf.Clients;

public interface IIntellishelfApiClient
{
    Task<string> LoginAsync(UserCredentials userCredentials);
    Task AddBook(Book book);
    Task<IEnumerable<Book>> GetBooksAsync();
    Task<Book> ParseBookFromTextAsync(string text);
}