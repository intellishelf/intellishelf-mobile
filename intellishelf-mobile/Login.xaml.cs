using Intellishelf.Clients;
using Intellishelf.Models;

namespace Intellishelf;

public partial class Login
{
    private readonly IIntellishelfApiClient _apiClient;

    public Login(IIntellishelfApiClient apiClient)
    {
        _apiClient = apiClient;
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Email and password are required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            var token = await _apiClient.LoginAsync(new UserCredentials(email, password));
            if (token != null && !string.IsNullOrWhiteSpace(token.AccessToken))
            {
                Preferences.Set("AccessToken", token.AccessToken);
                Preferences.Set("RefreshToken", token.RefreshToken);
                Preferences.Set("ExpiryDate", token.AccessTokenExpiry.ToString("o"));
                await Shell.Current.GoToAsync("//Books");
            }
            else
            {
                ErrorLabel.Text = "Invalid credentials.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Login failed: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
    }
}
