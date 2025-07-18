using Intellishelf.Models.Auth;

namespace Intellishelf.Clients;

public interface IIntellishelfAuthClient
{
    Task<AuthResult> LoginAsync(UserCredentials userCredentials);
    Task<AuthResult> RefreshAsync(string refreshToken);
}