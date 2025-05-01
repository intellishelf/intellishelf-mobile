using System.Text.Json;
using Intellishelf.Models;
using Intellishelf.Services;

namespace Intellishelf.Clients;

public class IntellishelfApiClient(HttpClient httpClient, AuthHandler authHandler)
    : BaseApiClient(httpClient, authHandler), IIntellishelfApiClient
{
    public async Task<AuthToken> LoginAsync(UserCredentials userCredentials)
    {
         var content = new StringContent(JsonSerializer.Serialize(userCredentials, InputJsonSerializerOptions), System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Login failed: {response.StatusCode}");
            throw new HttpRequestException($"Login failed with status code {response.StatusCode}");
        }

        var authToken = await ReadResponseAsync<AuthToken>(response);
        
        if (authToken == null)
        {
            throw new InvalidOperationException("Failed to deserialize authentication token");
        }

        return authToken;
    }

    public async Task AddBook(Book book)
    {
        await SendAsync(HttpMethod.Post, "books", book);
    }

    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        var response = await SendAsync(HttpMethod.Get, "books/all");
        var books = await ReadResponseAsync<IEnumerable<Book>>(response);
        return books ?? Array.Empty<Book>();
    }
    
    public async Task<PagedResult<Book>> GetBooksPagedAsync(int page, int pageSize, BookOrderBy orderBy = BookOrderBy.Added, bool ascending = true)
    {
        var parameters = new BookQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            OrderBy = orderBy,
            Ascending = ascending
        };
        
        var queryString = $"books?page={parameters.Page}&pageSize={parameters.PageSize}&orderBy={parameters.OrderBy}&ascending={parameters.Ascending}";
        var response = await SendAsync(HttpMethod.Get, queryString);
        var result = await ReadResponseAsync<PagedResult<Book>>(response);
        
        return result ?? new PagedResult<Book>(
            Array.Empty<Book>(),
            0,
            page,
            pageSize
        );
    }

    public async Task<Book> ParseBookFromTextAsync(string text)
    {
        var response = await SendAsync(HttpMethod.Post, "books/parse-text", new { text });
        var book = await ReadResponseAsync<Book>(response);
        
        if (book == null)
        {
            throw new InvalidOperationException("Failed to parse book from text");
        }
        
        return book;
    }
}