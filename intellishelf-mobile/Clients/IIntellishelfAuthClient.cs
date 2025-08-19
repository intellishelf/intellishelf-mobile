using Intellishelf.Common.TryResult;
using Intellishelf.Models.Auth;

namespace Intellishelf.Clients;

public interface IIntellishelfAuthClient
{
    Task<TryResult<AuthResult>> LoginAsync(UserCredentials userCredentials);
    Task<TryResult<AuthResult>> RefreshAsync(string refreshToken);
}
