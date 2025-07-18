using Intellishelf.Models.Auth;

namespace Intellishelf.Clients;

public class IntellishelfAuthClient(HttpClient httpClient)
    : IntellishelfBaseClient(httpClient), IIntellishelfAuthClient
{
    public async Task<AuthResult> LoginAsync(UserCredentials userCredentials) =>
        await SendAsync<AuthResult>(HttpMethod.Post, "auth/login", userCredentials);

    public async Task<AuthResult> RefreshAsync(string refreshToken) =>
        await SendAsync<AuthResult>(HttpMethod.Post, "auth/refresh", new { refreshToken });
}