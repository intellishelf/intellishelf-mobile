namespace Intellishelf.Models;

public class AuthToken
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime AccessTokenExpiry { get; set; }
}
