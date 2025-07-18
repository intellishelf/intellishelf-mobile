using Intellishelf.Models.Auth;

namespace Intellishelf.Services.Implementation;

public class AuthStorage : IAuthStorage
{
    public string? GetValidAccessToken() => IsAccessTokenValid()
        ? Preferences.Get(nameof(AuthResult.AccessToken), null)
        : null;

    public string? GetRefreshToken() => Preferences.Get(nameof(AuthResult.RefreshToken), null);

    public bool IsAccessTokenValid()
    {
        var expiryString = Preferences.Get(nameof(AuthResult.AccessTokenExpiry), string.Empty);
        if (string.IsNullOrEmpty(expiryString))
        {
            return false;
        }

        if (DateTime.TryParse(expiryString, out var expiry))
        {
            var isValid = expiry.ToUniversalTime() > DateTime.UtcNow.AddSeconds(30);

            return isValid;
        }

        return false;
    }

    public void StoreToken(AuthResult authToken)
    {
        Preferences.Set(nameof(AuthResult.AccessToken), authToken.AccessToken);
        Preferences.Set(nameof(AuthResult.RefreshToken), authToken.RefreshToken);
        Preferences.Set(nameof(AuthResult.AccessTokenExpiry), authToken.AccessTokenExpiry.ToString("o"));
    }

    public void ClearTokens()
    {
        Preferences.Remove(nameof(AuthResult.AccessToken));
        Preferences.Remove(nameof(AuthResult.RefreshToken));
        Preferences.Remove(nameof(AuthResult.AccessTokenExpiry));
    }
}