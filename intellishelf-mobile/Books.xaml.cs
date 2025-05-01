using Intellishelf.Clients;
using Intellishelf.ViewModels;

namespace Intellishelf;

public partial class Books
{
    private readonly BooksViewModel _viewModel;

    public Books(IIntellishelfApiClient client)
    {
        InitializeComponent();
        _viewModel = new BooksViewModel(client);
        BindingContext = _viewModel;

        Loaded += async (s, e) => await _viewModel.LoadInitialData();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await _viewModel.ResetAndRefresh();
        BooksRefreshView.IsRefreshing = false;
    }

    // private async void OnOpenCameraClicked(object sender, EventArgs e)
    // {
    //     try
    //     {
    //         IsBusy = true; // Show loader
    //
    //         // Ensure the device supports the camera
    //         if (MediaPicker.Default.IsCaptureSupported)
    //         {
    //             // Capture photo
    //             var photo = await MediaPicker.Default.CapturePhotoAsync();
    //
    //             if (photo != null)
    //             {
    //                 // Save the photo locally for processing
    //                 var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
    //
    //                 using (var stream = await photo.OpenReadAsync())
    //                 using (var fileStream = File.OpenWrite(localFilePath))
    //                 {
    //                     await stream.CopyToAsync(fileStream);
    //                 }
    //
    //                 // Call the API with the photo
    //                 await SendPhotoToApi(localFilePath);
    //             }
    //         }
    //         else
    //         {
    //             await DisplayAlert("Error", "Camera is not supported on this device.", "OK");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         await DisplayAlert("Error", ex.Message, "OK");
    //     }
    //     finally
    //     {
    //         IsBusy = false; // Hide loader
    //     }
    // }
    //
    // private async Task SendPhotoToApi(string filePath)
    // {
    //     try
    //     {
    //         var token = Preferences.Get("JwtToken", string.Empty);
    //
    //         using var httpClient = new HttpClient();
    //         using var fileStream = File.OpenRead(filePath);
    //         using var content = new MultipartFormDataContent
    //         {
    //             { new StreamContent(fileStream), "file", Path.GetFileName(filePath) }
    //         };
    //
    //         // Send the photo to your API
    //         httpClient.DefaultRequestHeaders.Authorization =
    //             new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    //
    //         var response =
    //             await httpClient.PostAsync("https://intellistest-api.azurewebsites.net/api/books/parse-image", content);
    //
    //         if (response.IsSuccessStatusCode)
    //         {
    //             // Parse the response
    //             var jsonResponse = await response.Content.ReadAsStringAsync();
    //             var formData = JsonSerializer.Deserialize<BookResponse>(jsonResponse);
    //
    //             await httpClient.PostAsync("https://intellistest-api.azurewebsites.net/api/books",
    //                 new StringContent(jsonResponse));
    //         }
    //         else
    //         {
    //             await DisplayAlert("Error", "Failed to upload photo.", "OK");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         await DisplayAlert("Error", ex.Message, "OK");
    //     }
    // }
}
