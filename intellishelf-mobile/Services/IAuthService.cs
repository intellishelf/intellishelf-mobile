using Intellishelf.Models;

namespace Intellishelf.Services;

public interface IAuthService
{
     Task<string> LoginAsync(UserCredentials userCredentials);

     void StoreToken(string token);
}