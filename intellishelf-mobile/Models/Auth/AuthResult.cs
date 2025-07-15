namespace Intellishelf.Models.Auth;

public class AuthResult
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime AccessTokenExpiry { get; init; }
}