using Intellishelf.Models.Auth;
using Intellishelf.Services;

namespace Intellishelf;

public partial class Login
{
    private readonly IIntellishelfApiClient _apiClient;
    private readonly IAuthStorage _tokenService;

    public Login(IIntellishelfApiClient apiClient, IAuthStorage tokenService)
    {
        _apiClient = apiClient;
        _tokenService = tokenService;
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

        var token = await _apiClient.LoginAsync(new UserCredentials(email, password));

        _tokenService.StoreToken(token);

        await Shell.Current.GoToAsync("//Books");
    }
}