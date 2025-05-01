using System.Net;
using System.Net.Http.Headers;
using Intellishelf.Models;
using System.Text.Json;

namespace Intellishelf.Services;

public class AuthHandler(ITokenService tokenService): DelegatingHandler
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private const string ApiBaseUrl = "https://intellishelf-test-fyhfe9bye5g2fud9.centralus-01.azurewebsites.net/api/";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Skip token handling for authentication endpoints
        if (request.RequestUri?.PathAndQuery.Contains("auth/login") == true ||
            request.RequestUri?.PathAndQuery.Contains("auth/register") == true ||
            request.RequestUri?.PathAndQuery.Contains("auth/refresh") == true)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // Add token if available
        await AddAuthorizationHeader(request, cancellationToken);

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If unauthorized, try to refresh the token and retry
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Acquire lock to prevent multiple refresh attempts
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                // Check if token was refreshed by another request while waiting
                await AddAuthorizationHeader(request, cancellationToken);

                // Check if token is still valid before attempting refresh
                if (!_tokenService.IsTokenValid())
                {
                    // Try to refresh the token
                    if (await RefreshTokenAsync(cancellationToken))
                    {
                        // Update the request with the new token
                        await AddAuthorizationHeader(request, cancellationToken);

                        // Retry the request
                        response = await base.SendAsync(request, cancellationToken);

                        // If still unauthorized after refresh, redirect to login
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            // Redirect to login page
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Shell.Current.GoToAsync("//Login");
                            });
                        }
                    }
                    else
                    {
                        // Failed to refresh token, redirect to login
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Shell.Current.GoToAsync("//Login");
                        });
                    }
                }
                else
                {
                    // Token is valid but still getting 401, redirect to login
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Shell.Current.GoToAsync("//Login");
                    });
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return response;
    }

    private async Task AddAuthorizationHeader(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenService.GetValidAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Preferences.Get("RefreshToken", string.Empty);
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(new { refreshToken }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiBaseUrl}auth/refresh");
            request.Content = content;

            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authToken = JsonSerializer.Deserialize<AuthToken>(responseContent, _jsonOptions);

                if (authToken != null)
                {
                    _tokenService.SetTokens(authToken);
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing token: {ex.Message}");
            return false;
        }
    }
}
