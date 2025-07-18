using Intellishelf.Models.Auth;

namespace Intellishelf.Services;

public interface IAuthStorage
{
    string? GetValidAccessToken();
    string? GetRefreshToken();
    bool IsAccessTokenValid();
    void StoreToken(AuthResult authToken);
    void ClearTokens();
}