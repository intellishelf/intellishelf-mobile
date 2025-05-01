namespace Intellishelf.Services;

public class TokenService : ITokenService
{
    public string GetValidAccessTokenAsync()
    {
        var token = Preferences.Get("AccessToken", string.Empty);
        var expiryString = Preferences.Get("ExpiryDate", string.Empty);

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(expiryString))
        {
            return string.Empty;
        }

        if (DateTime.TryParse(expiryString, out var expiry))
        {
            // If token expires in less than 30 seconds, try to refresh it
            if (expiry < DateTime.UtcNow.AddSeconds(30))
            {
                // Refresh token here if needed, using the same logic as AuthHandler
                return Preferences.Get("AccessToken", string.Empty);
            }
        }

        return token;
    }

    public bool IsTokenValid()
    {
        var expiryString = Preferences.Get("ExpiryDate", string.Empty);
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

    public void SetTokens(AuthToken authToken)
    {
        Preferences.Set("AccessToken", authToken.AccessToken);
        Preferences.Set("RefreshToken", authToken.RefreshToken);
        Preferences.Set("ExpiryDate", authToken.AccessTokenExpiry.ToString("o"));
    }

    public void ClearTokens()
    {
        Preferences.Remove("AccessToken");
        Preferences.Remove("RefreshToken");
        Preferences.Remove("ExpiryDate");
    }
}