using Intellishelf.Models;

namespace Intellishelf.Clients;

public interface IIntellishelfApiClient
{
    Task<string> GetTokenAsync(UserCredentials userCredentials);
}