using System.Net.Http.Headers;
using Intellishelf.Models;
using Intellishelf.Models.Books;

namespace Intellishelf.Clients;

public class IntellishelfApiClient(HttpClient httpClient)
    : IntellishelfBaseClient(httpClient), IIntellishelfApiClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<PagedResult<Book>> GetBooksPagedAsync(
        int page,
        int pageSize,
        BookOrderBy orderBy = BookOrderBy.Added,
        bool ascending = true)
    {
        var queryString = $"books?page={page}&pageSize={pageSize}&orderBy={orderBy}&ascending={ascending}";
        return await SendAsync<PagedResult<Book>>(HttpMethod.Get, queryString);
    }

    public async Task<Book> ParseBookFromTextAsync(string text) =>
        await SendAsync<Book>(HttpMethod.Post, "books/parse-text", new { text });



    public async Task AddBook(Book book)
    {
        using var formData = new MultipartFormDataContent();

        foreach (var (key, value) in book.ToFormData())
        {
            formData.Add(new StringContent(value), key);
        }

        if (book.CoverImage != null)
        {
            if (book.CoverImage.CanSeek)
            {
                book.CoverImage.Position = 0;
            }

            var imageContent = new StreamContent(book.CoverImage);

            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            formData.Add(imageContent, "ImageFile", "cover.jpg");
        }

        var response = await _httpClient.PostAsync("books", formData);

        response.EnsureSuccessStatusCode();
    }
}