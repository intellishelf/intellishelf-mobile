using System.Text.Json.Serialization;

namespace Intellishelf.Models;

public record UserCredentials(
    [property: JsonPropertyName("userName")]
    string Username,
    [property: JsonPropertyName("password")]
    string Password);