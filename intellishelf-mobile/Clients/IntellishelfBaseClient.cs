using System.Text;
using System.Text.Json;

namespace Intellishelf.Clients;

public abstract class IntellishelfBaseClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions InputJsonSerializerOptions = new()
        { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected async Task<T> SendAsync<T>(HttpMethod method, string relativeUrl, object? content = null)
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