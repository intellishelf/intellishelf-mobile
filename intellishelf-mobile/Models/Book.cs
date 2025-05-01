namespace Intellishelf.Models;

public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Authors { get; set; }
    public string Isbn { get; set; }
    public string Annotation { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    // public int? Pages { get; set; }
    public string ImageUrl { get; set; }
}