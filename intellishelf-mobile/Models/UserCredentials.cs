using System.Text.Json.Serialization;

namespace Intellishelf.Models;

public record UserCredentials(
    string UserName,
    string Password);