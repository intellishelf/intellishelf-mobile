using System.Text;
using System.Text.Json;
using Intellishelf.Common.TryResult;

namespace Intellishelf.Clients.Implementation;

public abstract class IntellishelfBaseClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions InputJsonSerializerOptions = new()
        { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected async Task<TryResult<T>> SendAsync<T>(HttpMethod method, string relativeUrl, object? content = null)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(content, InputJsonSerializerOptions), Encoding.UTF8, "application/json");
        return await SendAsync<T>(method, relativeUrl, httpContent);
    }

    protected async Task<TryResult<T>> SendAsync<T>(HttpMethod method, string relativeUrl, HttpContent httpContent)
    {
        try
        {
            var request = new HttpRequestMessage(method, relativeUrl)
            {
                Content = httpContent
            };

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var problemDetails = await ReadResponseAsync<ProblemDetails?>(response);

                var errorMessage = problemDetails?.Errors == null
                    ? problemDetails?.Title
                    : string.Join(", ", problemDetails.Errors.SelectMany(e => e.Value));

                return TryResult.Failure<T>(errorMessage ?? "Unknown error");
            }

            var result = await ReadResponseAsync<T>(response);
            return TryResult.Success(result);
        }
        catch (Exception)
        {
            return TryResult.Failure<T>("Network error");
        }
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response) =>
        JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), InputJsonSerializerOptions)!;
}