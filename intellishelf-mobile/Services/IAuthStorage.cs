using Intellishelf.Models.Auth;

namespace Intellishelf.Services;

public interface IAuthStorage
{
    string? GetValidAccessToken();
    bool IsTokenValid();
    string GetUserId();
    void StoreToken(AuthResult authToken);
}