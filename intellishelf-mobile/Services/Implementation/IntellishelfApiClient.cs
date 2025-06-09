using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Intellishelf.Models;
using Intellishelf.Models.Auth;
using Intellishelf.Models.Books;

namespace Intellishelf.Services.Implementation;

public class IntellishelfApiClient(HttpClient httpClient)
    : IIntellishelfApiClient
{
    private static readonly JsonSerializerOptions InputJsonSerializerOptions = new()
        { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<AuthResult> LoginAsync(UserCredentials userCredentials) =>
        await SendAsync<AuthResult>(HttpMethod.Post, "auth/login", userCredentials);

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

    public async Task<byte[]> GetImageContentAsync(string userId, string fileName) =>
        await (await httpClient.GetAsync($"{httpClient.BaseAddress}users/{userId}/files/{fileName}")).Content
            .ReadAsByteArrayAsync();

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

        var response = await httpClient.PostAsync("books", formData);

        response.EnsureSuccessStatusCode();
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string relativeUrl, object? content = null)
    {
        var request = new HttpRequestMessage(method, relativeUrl);

        if (content != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(content, InputJsonSerializerOptions),
                Encoding.UTF8, "application/json");
        }

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await ReadResponseAsync<T>(response);
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response) =>
        JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), InputJsonSerializerOptions);
}