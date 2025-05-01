using Intellishelf.Clients;
using Intellishelf.Models;

namespace Intellishelf.Services;

public class AuthService(IIntellishelfApiClient client) : IAuthService
{
    public async Task<string> LoginAsync(UserCredentials userCredentials) =>
        await client.LoginAsync(userCredentials);

    public void StoreToken(string token) =>
        Preferences.Set("JwtToken", token);
}