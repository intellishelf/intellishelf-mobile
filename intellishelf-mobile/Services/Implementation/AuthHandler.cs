using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Intellishelf.Infra;
using Intellishelf.Models.Auth;
using Microsoft.Extensions.Options;

namespace Intellishelf.Services.Implementation;

public class AuthHandler(IOptions<ApiSettings> apiSettings, IAuthStorage tokenService)
    : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
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
                if (!tokenService.IsTokenValid())
                {
                    // Try to refresh the token
                    if (await RefreshTokenAsync(cancellationToken))
                    {
                        // Update the request with the new token
                        await AddAuthorizationHeader(request, cancellationToken);

                        // Retry the request
                        response = await base.SendAsync(request, cancellationToken);

                        // If still unauthorized after refresh, clear tokens and redirect to login
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            tokenService.ClearTokens();
                            MainThread.BeginInvokeOnMainThread(() => { Shell.Current.GoToAsync("//Login"); });
                            
                            // Return a custom response to prevent client from seeing 401
                            return CreateAuthFailureResponse();
                        }
                    }
                    else
                    {
                        // Failed to refresh token, clear tokens and redirect to login
                        tokenService.ClearTokens();
                        MainThread.BeginInvokeOnMainThread(() => { Shell.Current.GoToAsync("//Login"); });
                        
                        // Return a custom response to prevent client from seeing 401
                        return CreateAuthFailureResponse();
                    }
                }
                else
                {
                    // Token is valid but still getting 401, clear tokens and redirect to login
                    tokenService.ClearTokens();
                    MainThread.BeginInvokeOnMainThread(() => { Shell.Current.GoToAsync("//Login"); });
                    
                    // Return a custom response to prevent client from seeing 401
                    return CreateAuthFailureResponse();
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
        var token = tokenService.GetValidAccessToken();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = tokenService.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        var content = new StringContent(
            JsonSerializer.Serialize(new { refreshToken },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            System.Text.Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{apiSettings.Value.BaseUrl}/auth/refresh");
        request.Content = content;

        var response = await base.SendAsync(request, cancellationToken);

        // Check if refresh was successful
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var authToken = JsonSerializer.Deserialize<AuthResult>(responseContent, _jsonOptions);

        if (authToken != null)
        {
            tokenService.StoreToken(authToken);
            return true;
        }

        return false;
    }

    private HttpResponseMessage CreateAuthFailureResponse()
    {
        // Create a successful response to prevent EnsureSuccessStatusCode from throwing
        // We're redirecting to login anyway, so this prevents the client crash
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new StringContent("{}", 
            System.Text.Encoding.UTF8, "application/json");
        return response;
    }
}