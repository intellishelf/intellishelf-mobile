using Intellishelf.Models.Auth;

namespace Intellishelf.Services.Implementation;

public class AuthStorage : IAuthStorage
{
    public string? GetValidAccessToken() => IsTokenValid()
        ? Preferences.Get(nameof(AuthResult.AccessToken), null)
        : null;

    public bool IsTokenValid()
    {
        var expiryString = Preferences.Get(nameof(AuthResult.AccessTokenExpiry), string.Empty);
        if (string.IsNullOrEmpty(expiryString))
        {
            return false;
        }

        if (DateTime.TryParse(expiryString, out var expiry))
        {
            return expiry > DateTime.UtcNow.AddSeconds(30);
        }

        return false;
    }

    public string GetUserId() => Preferences.Get(nameof(AuthResult.UserId), string.Empty);

    public void StoreToken(AuthResult authToken)
    {
        Preferences.Set(nameof(AuthResult.UserId), authToken.UserId);
        Preferences.Set(nameof(AuthResult.AccessToken), authToken.AccessToken);
        Preferences.Set(nameof(AuthResult.RefreshToken), authToken.RefreshToken);
        Preferences.Set(nameof(AuthResult.AccessTokenExpiry), authToken.AccessTokenExpiry.ToString("o"));
    }
}