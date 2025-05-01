using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Intellishelf.Models;

namespace Intellishelf.Clients;

public class IntellishelfApiClient : IIntellishelfApiClient
{
    private const string BaseUrl = "https://intellishelf-api-test2.azurewebsites.net/api/";

    private static readonly JsonSerializerOptions OutputJsonSerializerOptions =
        new() { PropertyNameCaseInsensitive = true };

    private static readonly JsonSerializerOptions InputJsonSerializerOptions = new()
        { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };


    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(BaseUrl)
    };

    public async Task<string> LoginAsync(UserCredentials userCredentials)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(userCredentials, InputJsonSerializerOptions), Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("auth/login", content);

            if (!response.IsSuccessStatusCode) throw new Exception("Failed to get token");

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AuthToken>(responseContent, OutputJsonSerializerOptions)?.Token;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task AddBook(Book book)
    {
        try
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(book, InputJsonSerializerOptions), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "books")
            {
                Content = stringContent
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("JwtToken", null));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) throw new Exception("Failed to add a book");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "books");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("JwtToken", null));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) throw new Exception("Failed to get token");

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IEnumerable<Book>>(responseContent, OutputJsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Book> ParseBookFromTextAsync(string text)
    {
        try
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(new
            {
                text
            }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "books/parse-text")
            {
                Content = stringContent
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("JwtToken", null));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) throw new Exception("Failed to get token");

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Book>(responseContent, OutputJsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}