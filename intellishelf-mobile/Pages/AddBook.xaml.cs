using Foundation;
using Intellishelf.Models.Books;
using Intellishelf.Services;
using UIKit;
using Vision;

namespace Intellishelf.Pages;

public partial class AddBook
{
    private readonly IIntellishelfApiClient _client;
    private byte[]? _selectedImageData;

    public AddBook(IIntellishelfApiClient client)
    {
        _client = client;
        InitializeComponent();
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        var result = await MediaPicker.PickPhotoAsync();

        if (result == null) return;

        await using var stream = await result.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        _selectedImageData = memoryStream.ToArray();

        CoverImage.Source = ImageSource.FromStream(() => new MemoryStream(_selectedImageData));
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var book = new Book
        {
            Title = Title.Text,
            Authors = Authors.Text,
            Publisher = Publisher.Text,
            Isbn = Isbn.Text,
            CoverImage = _selectedImageData != null ? new MemoryStream(_selectedImageData) : null
        };

        await _client.AddBook(book);

        await DisplayAlert("Added", "Book has been added", "OK");

        await Shell.Current.GoToAsync("//Books");
    }

    private async void OnOpenCameraClicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.Default.CapturePhotoAsync();

        if (photo == null) return;
        var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

        await using (var stream = await photo.OpenReadAsync())
        await using (var fileStream = File.OpenWrite(localFilePath))
        {
            await stream.CopyToAsync(fileStream);
        }

        await SendPhotoToApi(localFilePath);
    }

    private async Task SendPhotoToApi(string filePath)
    {
        var parsedText = await ExtractTextAsync(filePath);

        var book = await _client.ParseBookFromTextAsync(parsedText);

        Title.Text = book.Title;
        Authors.Text = book.Authors;
        Publisher.Text = book.Publisher;
        Isbn.Text = book.Isbn;
    }

    private async Task<string> ExtractTextAsync(string filePath)
    {
        var image = UIImage.FromFile(filePath);

        var imageRequestHandler = new VNImageRequestHandler(image.CGImage, options: new NSDictionary());
        var recognizedText = string.Empty;

        var textRequest = new VNRecognizeTextRequest((request, _) =>
        {
            var results = request.GetResults<VNRecognizedTextObservation>();
            if (results != null)
            {
                recognizedText = string.Join("\n", results
                    .SelectMany(obs => obs.TopCandidates(1))
                    .Select(candidate => candidate.String));
            }
        })
        {
            RecognitionLevel = VNRequestTextRecognitionLevel.Accurate
        };

        await Task.Run(() =>
        {
            imageRequestHandler.Perform([textRequest], out var performError);

            Console.WriteLine($"Error performing the request: {performError?.LocalizedDescription}");
        });

        return recognizedText;
    }
}