using System.Text.Json.Serialization;

namespace Intellishelf.Models;

public class Book
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("authors")]
    public string Authors { get; set; }

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; }
}