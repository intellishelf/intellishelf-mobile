using System.Text;
using System.Text.Json;
using Intellishelf.Models;

namespace Intellishelf.Clients;

public class IntellishelfApiClient : IIntellishelfApiClient
{
    private const string BaseUrl = "https://intellistest-api.azurewebsites.net/api/";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(BaseUrl)
    };

    public async Task<string> GetTokenAsync(UserCredentials userCredentials)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(userCredentials), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/login", content);

            if (!response.IsSuccessStatusCode) throw new Exception("Failed to get token");

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AuthToken>(responseContent, JsonSerializerOptions)?.Token;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}