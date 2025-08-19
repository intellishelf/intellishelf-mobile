using System.Net.Http.Headers;
using Intellishelf.Common.TryResult;
using Intellishelf.Models;
using Intellishelf.Models.Books;

namespace Intellishelf.Clients.Implementation;

public class IntellishelfApiClient(HttpClient httpClient)
    : IntellishelfBaseClient(httpClient), IIntellishelfApiClient
{
    public async Task<TryResult<PagedResult<Book>>> GetBooksPagedAsync(
        int page,
        int pageSize,
        BookOrderBy orderBy = BookOrderBy.Added,
        bool ascending = true)
    {
        var queryString = $"/books?page={page}&pageSize={pageSize}&orderBy={orderBy}&ascending={ascending}";
        return await SendAsync<PagedResult<Book>>(HttpMethod.Get, queryString);
    }

    public async Task<TryResult<Book>> ParseBookFromTextAsync(string text) =>
        await SendAsync<Book>(HttpMethod.Post, "/books/parse-text", new { text });

    public async Task<TryResult> AddBook(Book book)
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

        await SendAsync<Book>(HttpMethod.Post, "/books", formData);

        return TryResult.Success();
    }
}