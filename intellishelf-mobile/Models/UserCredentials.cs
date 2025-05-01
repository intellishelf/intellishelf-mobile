using System.Text.Json.Serialization;

namespace Intellishelf.Models;

public record UserCredentials(
    string Email,
    string Password);
