using Foundation;
using Intellishelf.Clients;
using Intellishelf.Models;
using UIKit;
using Vision;

namespace Intellishelf;

public partial class AddBook
{
    private readonly IIntellishelfApiClient _client;

    public AddBook(IIntellishelfApiClient client)
    {
        _client = client;
        InitializeComponent();
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var book = new Book
        {
            Title = Title.Text,
            Authors = Author.Text,
            Publisher = Publisher.Text,
            Isbn = Isbn.Text,
        };

        await _client.AddBook(book);

        await DisplayAlert("Added", "Book has been added", "OK");

        await Shell.Current.GoToAsync("//Books");
    }


    private async void OnOpenCameraClicked(object sender, EventArgs e)
    {
        try
        {
            Indicator.IsVisible = true;
            Indicator.IsRunning = true;

            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo == null) return;
                var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using (var stream = await photo.OpenReadAsync())
                using (var fileStream = File.OpenWrite(localFilePath))
                {
                    await stream.CopyToAsync(fileStream);
                }

                await SendPhotoToApi(localFilePath);
            }
            else
            {
                await DisplayAlert("Error", "Camera is not supported on this device.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task SendPhotoToApi(string filePath)
    {
        try
        {
            var parsedText = await ExtractTextAsync(filePath);

            var book = await _client.ParseBookFromTextAsync(parsedText);

            Title.Text = book.Title;
            Author.Text = book.Authors;
            Publisher.Text = book.Publisher;
            Isbn.Text = book.Isbn;

            Indicator.IsVisible = false;
            Indicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task<string> ExtractTextAsync(string filePath)
    {
        var image = UIImage.FromFile(filePath);

        if (image?.CGImage == null)
        {
            throw new InvalidOperationException("Image could not be loaded.");
        }

        var imageRequestHandler = new VNImageRequestHandler(image.CGImage, options: new NSDictionary());
        var recognizedText = string.Empty;

        var textRequest = new VNRecognizeTextRequest((request, error) =>
        {
            if (error != null)
            {
                Console.WriteLine($"Error during text recognition: {error.LocalizedDescription}");
                return;
            }

            var results = request.GetResults<VNRecognizedTextObservation>();
            if (results != null)
            {
                recognizedText = string.Join("\n", results
                    .SelectMany(obs => obs.TopCandidates(1))
                    .Select(candidate => candidate.String));
            }
        })
        {
            RecognitionLevel =VNRequestTextRecognitionLevel.Accurate
        };

        await Task.Run(() =>
        {
            imageRequestHandler.Perform([textRequest], out var performError);

            Console.WriteLine($"Error performing the request: {performError?.LocalizedDescription}");
        });

        return recognizedText;
    }
}