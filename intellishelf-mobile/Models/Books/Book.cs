using System.Text.Json.Serialization;

namespace Intellishelf.Models.Books;

public class Book
{
    public string Id { get; set; }
    public required string Title { get; init; }

    public string? Annotation { get; init; }
    public string? Authors { get; init; }
    public string? Description { get; init; }
    public string? FileName { get; init; }
    public string? Isbn { get; init; }
    public int? Pages { get; init; }
    public DateTime? PublicationDate { get; init; }
    public string? Publisher { get; init; }
    public string[]? Tags { get; init; }
    public string? CoverUrl { get; set; }

    [JsonIgnore]
    public Stream? CoverImage { get; set; }
    
    [JsonIgnore]
    public ImageSource? CoverImageSource { get; set; }

    public Dictionary<string, string> ToFormData()
    {
        var formData = new Dictionary<string, string>
        {
            { "Title", Title }
        };

        if (!string.IsNullOrEmpty(Authors)) formData["Authors"] = Authors;
        if (!string.IsNullOrEmpty(Publisher)) formData["Publisher"] = Publisher;
        if (!string.IsNullOrEmpty(Isbn)) formData["Isbn"] = Isbn;
        if (!string.IsNullOrEmpty(Annotation)) formData["Annotation"] = Annotation;
        if (!string.IsNullOrEmpty(Description)) formData["Description"] = Description;
        if (Pages.HasValue) formData["Pages"] = Pages.Value.ToString();
        if (PublicationDate.HasValue) formData["PublicationDate"] = PublicationDate.Value.ToString("O");
        if (Tags is { Length: > 0 }) formData["Tags"] = string.Join(",", Tags);

        return formData;
    }
}