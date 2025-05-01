namespace Intellishelf.Services;

public interface ITokenService
{
    string GetValidAccessTokenAsync();
    bool IsTokenValid();
    void SetTokens(string accessToken, string refreshToken, DateTime expiry);
    void ClearTokens();
}