namespace Intellishelf.Models;

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
}