using System.Net.Http.Headers;
using Intellishelf.Clients;
using Intellishelf.Infra;

namespace Intellishelf.Services.Implementation;

public class AuthHandler(IAuthStorage tokenService, IIntellishelfAuthClient authClient)
    : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!tokenService.IsAccessTokenValid())
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                var refreshResult = await authClient.RefreshAsync(tokenService.GetRefreshToken());

                tokenService.StoreToken(refreshResult);
            }
            catch (Exception)
            {
                tokenService.ClearTokens();

                throw new UserSessionExpiredException();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",  tokenService.GetValidAccessToken());

        return await base.SendAsync(request, cancellationToken);
    }
}