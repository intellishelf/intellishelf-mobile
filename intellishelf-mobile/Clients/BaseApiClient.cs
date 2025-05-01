using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Intellishelf.Services;

namespace Intellishelf.Clients;

public abstract class BaseApiClient
{
    protected const string BaseUrl = "https://intellishelf-test-fyhfe9bye5g2fud9.centralus-01.azurewebsites.net/api/";

    protected static readonly JsonSerializerOptions OutputJsonSerializerOptions =
        new() { PropertyNameCaseInsensitive = true };

    protected static readonly JsonSerializerOptions InputJsonSerializerOptions = new()
        { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected readonly HttpClient HttpClient;
    private readonly AuthHandler _authHandler;

    protected BaseApiClient(HttpClient httpClient, AuthHandler authHandler)
    {
        HttpClient = httpClient;
        _authHandler = authHandler;
        HttpClient.BaseAddress = new Uri(BaseUrl);
    }

    protected async Task<HttpResponseMessage> SendAsync(HttpMethod method, string relativeUrl, object? content = null)
    {
        var request = new HttpRequestMessage(method, relativeUrl);

        if (content != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(content, InputJsonSerializerOptions), Encoding.UTF8, "application/json");
        }

        try
        {
            var response = await HttpClient.SendAsync(request);
            // Basic error handling
            if (!response.IsSuccessStatusCode)
            {
                // Log or handle specific status codes if needed
                Console.WriteLine($"API Request Failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                throw new HttpRequestException($"Request to {relativeUrl} failed with status code {response.StatusCode}");
            }
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during API call to {relativeUrl}: {e}");
            throw;
        }
    }

    protected async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, OutputJsonSerializerOptions);
    }
}
