using System.Text.Json.Serialization;

public class BookResponse
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Authors { get; set; }
    public DateTime PublicationDate { get; set; }
    public string Isbn { get; set; }
    public string Annotation { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public int Pages { get; set; }
    public string ImageUrl { get; set; }
}